using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IOrderRepository orders)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(query.Id, cancellationToken);

        if (order is null)
            return Result.Fail<OrderResponse>(OrderErrors.NotFound(query.Id));

        return Result.Ok(order.ToResponse());
    }
}
