using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Products.Events;

/// <summary>
/// Nao carrega o id: o produto ainda nao foi persistido quando o evento e
/// publicado. O SKU e a chave natural e ja e conhecido aqui.
/// </summary>
public sealed record ProductRegistered(string Sku, DateTime OccurredOnUtc) : IDomainEvent;
