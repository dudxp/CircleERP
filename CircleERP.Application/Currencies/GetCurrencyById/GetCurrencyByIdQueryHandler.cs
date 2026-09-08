using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Currencies;
using FluentResults;

namespace CircleERP.Application.Currencies.GetCurrencyById;

internal sealed class GetCurrencyByIdQueryHandler(ICurrencyRepository currencies)
    : IQueryHandler<GetCurrencyByIdQuery, CurrencyResponse>
{
    public async Task<Result<CurrencyResponse>> Handle(
        GetCurrencyByIdQuery query,
        CancellationToken cancellationToken)
    {
        var currency = await currencies.GetByIdAsync(query.Id, cancellationToken);

        if (currency is null)
            return Result.Fail<CurrencyResponse>(CurrencyErrors.NotFound(query.Id));

        return Result.Ok(currency.ToResponse());
    }
}
