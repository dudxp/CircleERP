using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Customers;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler(
    IOrderRepository orders,
    ICustomerRepository customers)
    : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderSummaryResponse>>
{
    public async Task<Result<IReadOnlyList<OrderSummaryResponse>>> Handle(
        GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await orders.GetAllAsync(cancellationToken);

        // Nomes resolvidos em uma consulta so, e nao uma por pedido.
        var customerIds = result.Select(order => order.CustomerId).Distinct().ToArray();

        var customersById = (await customers.GetByIdsAsync(customerIds, cancellationToken))
            .ToDictionary(customer => customer.Id);

        return Result.Ok<IReadOnlyList<OrderSummaryResponse>>(
            [.. result.Select(order => order.ToSummary(customersById.NameOf(order.CustomerId)))]);
    }
}
