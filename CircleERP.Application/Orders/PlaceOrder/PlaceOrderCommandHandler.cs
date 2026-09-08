using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Abstractions.Time;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.PlaceOrder;

internal sealed class PlaceOrderCommandHandler(
    IOrderRepository orders,
    IDateTimeProvider dateTime,
    IUnitOfWork unitOfWork) : ICommandHandler<PlaceOrderCommand>
{
    public async Task<Result> Handle(
        PlaceOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            return Result.Fail(OrderErrors.NotFound(command.OrderId));

        // Pedido sem itens e pedido ja confirmado sao recusados pelo agregado:
        // o handler nao repete essas regras.
        order.Place(dateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
