using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Currencies.Events;

public sealed record CurrencyRateChanged(
    int CurrencyId,
    decimal PreviousRate,
    decimal CurrentRate,
    DateTime OccurredOnUtc) : IDomainEvent;
