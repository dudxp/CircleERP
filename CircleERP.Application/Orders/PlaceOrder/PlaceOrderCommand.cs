using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.PlaceOrder;

/// <summary>Confirma o pedido, encerrando a edicao dos itens.</summary>
public sealed record PlaceOrderCommand(int OrderId) : ICommand;
