using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Currencies;
using FluentResults;

namespace CircleERP.Application.Currencies.RegisterCurrency;

internal sealed class RegisterCurrencyCommandHandler(
    ICurrencyRepository currencies,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterCurrencyCommand, int>
{
    public async Task<Result<int>> Handle(
        RegisterCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var code = CurrencyCode.Create(command.Code);

        if (await currencies.ExistsWithCodeAsync(code, cancellationToken))
            return Result.Fail<int>(CurrencyErrors.CodeAlreadyRegistered(code.Value));

        var currency = Currency.Register(
            code,
            CurrencyDescription.Create(command.Description),
            ExchangeRate.Create(command.Rate),
            CurrencySymbol.CreateOrNull(command.Symbol));

        currencies.Add(currency);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(currency.Id);
    }
}
