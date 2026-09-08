using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.RemoveOrderItem;

internal sealed class RemoveOrderItemCommandHandler(
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveOrderItemCommand>
{
    public async Task<Result> Handle(
        RemoveOrderItemCommand command,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            return Result.Fail(OrderErrors.NotFound(command.OrderId));

        order.RemoveItem(command.ItemId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
