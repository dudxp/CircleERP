using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Currencies;
using FluentResults;

namespace CircleERP.Application.Currencies.RemoveCurrency;

internal sealed class RemoveCurrencyCommandHandler(
    ICurrencyRepository currencies,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveCurrencyCommand>
{
    public async Task<Result> Handle(
        RemoveCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var currency = await currencies.GetByIdAsync(command.Id, cancellationToken);

        if (currency is null)
            return Result.Fail(CurrencyErrors.NotFound(command.Id));

        currencies.Remove(currency);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
