using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Customers.SetCustomerStatus;

/// <summary>
/// Ativa ou inativa um cliente.
/// </summary>
/// <remarks>
/// Nao existe exclusao de cliente: inativar preserva os pedidos antigos, que
/// precisam seguir legiveis. Um comando com o alvo explicito, em vez de um
/// "alternar", evita que duas requisicoes simultaneas se cancelem.
/// </remarks>
public sealed record SetCustomerStatusCommand(int Id, bool Active) : ICommand;
