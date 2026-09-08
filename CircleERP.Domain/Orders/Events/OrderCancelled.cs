using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Orders.Events;

public sealed record OrderCancelled(
    int OrderId,
    OrderStatus PreviousStatus,
    DateTime OccurredOnUtc) : IDomainEvent;
