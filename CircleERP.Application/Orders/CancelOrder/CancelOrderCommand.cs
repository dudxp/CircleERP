using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.CancelOrder;

public sealed record CancelOrderCommand(int OrderId) : ICommand;
