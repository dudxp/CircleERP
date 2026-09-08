using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Abstractions.Time;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Orders;
using FluentResults;

namespace CircleERP.Application.Orders.OpenOrder;

internal sealed class OpenOrderCommandHandler(
    IOrderRepository orders,
    ICurrencyRepository currencies,
    IDateTimeProvider dateTime,
    IUnitOfWork unitOfWork) : ICommandHandler<OpenOrderCommand, int>
{
    public async Task<Result<int>> Handle(
        OpenOrderCommand command,
        CancellationToken cancellationToken)
    {
        var currencyCode = CurrencyCode.Create(command.CurrencyCode);

        // O pedido guarda so o codigo da moeda, mas a moeda precisa existir:
        // essa e uma regra entre agregados, e por isso vive no caso de uso.
        var currency = await currencies.GetByCodeAsync(currencyCode, cancellationToken);

        if (currency is null)
            return Result.Fail<int>(OrderErrors.CurrencyNotRegistered(currencyCode.Value));

        var order = Order.Open(
            CustomerName.Create(command.Customer),
            currencyCode,
            dateTime.UtcNow);

        orders.Add(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(order.Id);
    }
}
