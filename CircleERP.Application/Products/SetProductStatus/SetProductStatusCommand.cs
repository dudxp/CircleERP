using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Products.SetProductStatus;

/// <summary>
/// Ativa ou inativa um produto. Nao existe exclusao: as linhas de pedido que ja
/// o venderam precisam seguir legiveis.
/// </summary>
public sealed record SetProductStatusCommand(int Id, bool Active) : ICommand;
