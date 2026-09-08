using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.ChangeOrderItemQuantity;

public sealed record ChangeOrderItemQuantityCommand(int OrderId, int ItemId, int Quantity)
    : ICommand;
