using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Currencies;
using FluentResults;

namespace CircleERP.Application.Currencies.GetCurrencies;

internal sealed class GetCurrenciesQueryHandler(ICurrencyRepository currencies)
    : IQueryHandler<GetCurrenciesQuery, IReadOnlyList<CurrencyResponse>>
{
    public async Task<Result<IReadOnlyList<CurrencyResponse>>> Handle(
        GetCurrenciesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await currencies.GetAllAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<CurrencyResponse>>(
            [.. result.Select(currency => currency.ToResponse())]);
    }
}
