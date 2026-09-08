using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Orders.Events;

/// <summary>
/// Pedido confirmado pelo cliente. Ponto de entrada natural para reserva de
/// estoque, emissao de documento fiscal ou notificacao.
/// </summary>
public sealed record OrderPlaced(
    int OrderId,
    string Customer,
    string Currency,
    decimal Total,
    DateTime OccurredOnUtc) : IDomainEvent;
