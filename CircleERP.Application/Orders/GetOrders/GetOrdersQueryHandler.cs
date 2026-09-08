using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler(IOrderRepository orders)
    : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderSummaryResponse>>
{
    public async Task<Result<IReadOnlyList<OrderSummaryResponse>>> Handle(
        GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await orders.GetAllAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<OrderSummaryResponse>>(
            [.. result.Select(order => order.ToSummary())]);
    }
}
