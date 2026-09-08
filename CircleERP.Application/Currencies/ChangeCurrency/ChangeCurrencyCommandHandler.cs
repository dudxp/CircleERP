using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Currencies;
using FluentResults;

namespace CircleERP.Application.Currencies.ChangeCurrency;

internal sealed class ChangeCurrencyCommandHandler(
    ICurrencyRepository currencies,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangeCurrencyCommand>
{
    public async Task<Result> Handle(
        ChangeCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var currency = await currencies.GetByIdAsync(command.Id, cancellationToken);

        if (currency is null)
            return Result.Fail(CurrencyErrors.NotFound(command.Id));

        currency.ChangeDescription(CurrencyDescription.Create(command.Description));
        currency.ChangeRate(ExchangeRate.Create(command.Rate));
        currency.ChangeSymbol(CurrencySymbol.CreateOrNull(command.Symbol));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
