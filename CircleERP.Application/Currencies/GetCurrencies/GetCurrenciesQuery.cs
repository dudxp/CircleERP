using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Currencies.GetCurrencies;

public sealed record GetCurrenciesQuery : IQuery<IReadOnlyList<CurrencyResponse>>;
