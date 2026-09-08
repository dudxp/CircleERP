using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.RemoveOrderItem;

public sealed record RemoveOrderItemCommand(int OrderId, int ItemId) : ICommand;
