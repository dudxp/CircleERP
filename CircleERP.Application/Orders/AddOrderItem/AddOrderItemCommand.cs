using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.AddOrderItem;

/// <summary>
/// Adiciona uma linha ao pedido.
/// </summary>
/// <remarks>
/// A descricao nao entra: ela e copiada do produto no momento da venda. O preco
/// entra porque e negociavel -- o cadastro sugere, o vendedor decide -- e vem so
/// como quantia, ja que a moeda e a do pedido.
/// </remarks>
public sealed record AddOrderItemCommand(
    int OrderId,
    int ProductId,
    int Quantity,
    decimal UnitPrice) : ICommand<int>;
