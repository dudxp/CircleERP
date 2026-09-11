using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.OpenOrder;

/// <summary>
/// Abre um pedido em rascunho. Os itens entram depois, um comando por vez.
/// </summary>
public sealed record OpenOrderCommand(int CustomerId, string CurrencyCode) : ICommand<int>;
