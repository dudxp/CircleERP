using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Abstractions.Time;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.CancelOrder;

internal sealed class CancelOrderCommandHandler(
    IOrderRepository orders,
    IDateTimeProvider dateTime,
    IUnitOfWork unitOfWork) : ICommandHandler<CancelOrderCommand>
{
    public async Task<Result> Handle(
        CancelOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            return Result.Fail(OrderErrors.NotFound(command.OrderId));

        order.Cancel(dateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
