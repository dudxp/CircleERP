using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.ChangeOrderItemQuantity;

internal sealed class ChangeOrderItemQuantityCommandHandler(
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangeOrderItemQuantityCommand>
{
    public async Task<Result> Handle(
        ChangeOrderItemQuantityCommand command,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            return Result.Fail(OrderErrors.NotFound(command.OrderId));

        order.ChangeItemQuantity(command.ItemId, Quantity.Create(command.Quantity));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
