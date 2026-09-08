using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Currencies.GetCurrencyById;

public sealed record GetCurrencyByIdQuery(int Id) : IQuery<CurrencyResponse>;
