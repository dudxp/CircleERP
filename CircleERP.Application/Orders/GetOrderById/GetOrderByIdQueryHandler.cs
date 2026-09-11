using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Customers;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(
    IOrderRepository orders,
    ICustomerRepository customers)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(query.Id, cancellationToken);

        if (order is null)
            return Result.Fail<OrderResponse>(OrderErrors.NotFound(query.Id));

        var customer = await customers.GetByIdAsync(order.CustomerId, cancellationToken);

        return Result.Ok(order.ToResponse(customer?.Name.Value ?? $"(cliente {order.CustomerId})"));
    }
}
