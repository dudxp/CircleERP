using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.AddOrderItem;

/// <summary>
/// O preco unitario e so a quantia: a moeda vem do pedido, nao da requisicao.
/// </summary>
public sealed record AddOrderItemCommand(
    int OrderId,
    string Description,
    int Quantity,
    decimal UnitPrice) : ICommand<int>;
