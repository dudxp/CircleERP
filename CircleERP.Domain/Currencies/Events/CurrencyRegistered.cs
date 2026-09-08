using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Currencies.Events;

/// <summary>
/// Nao carrega o id: no momento em que a moeda e registrada ela ainda nao foi
/// persistida, e o id gerado pelo banco so existe apos o INSERT. O codigo e a
/// chave natural e ja e conhecido aqui.
/// </summary>
public sealed record CurrencyRegistered(string Code, DateTime OccurredOnUtc) : IDomainEvent;
