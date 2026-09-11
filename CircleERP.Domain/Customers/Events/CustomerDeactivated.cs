using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Customers.Events;

public sealed record CustomerDeactivated(int CustomerId, DateTime OccurredOnUtc) : IDomainEvent;
