using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.AddOrderItem;

internal sealed class AddOrderItemCommandHandler(
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : ICommandHandler<AddOrderItemCommand, int>
{
    public async Task<Result<int>> Handle(
        AddOrderItemCommand command,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            return Result.Fail<int>(OrderErrors.NotFound(command.OrderId));

        // Se o pedido nao estiver em rascunho, o proprio agregado recusa.
        var item = order.AddItem(
            ItemDescription.Create(command.Description),
            Quantity.Create(command.Quantity),
            command.UnitPrice);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(item.Id);
    }
}
