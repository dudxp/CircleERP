using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Customers.Events;

/// <summary>
/// Nao carrega o id: o cliente ainda nao foi persistido quando o evento e
/// publicado. O documento e a chave natural e ja e conhecido aqui.
/// </summary>
public sealed record CustomerRegistered(string Document, DateTime OccurredOnUtc) : IDomainEvent;
