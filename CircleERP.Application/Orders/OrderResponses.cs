namespace CircleERP.Application.Orders;

/// <summary>Pedido na listagem: sem os itens, que so a tela de detalhe usa.</summary>
public sealed record OrderSummaryResponse(
    int Id,
    string Customer,
    string Currency,
    string Status,
    decimal Total,
    int ItemCount,
    DateTime CreatedOnUtc);

/// <summary>Pedido completo, com as linhas.</summary>
public sealed record OrderResponse(
    int Id,
    string Customer,
    string Currency,
    string Status,
    decimal Total,
    DateTime CreatedOnUtc,
    DateTime? PlacedOnUtc,
    IReadOnlyList<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    int Id,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);
