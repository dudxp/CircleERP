namespace CircleERP.Domain.Orders;

/// <summary>
/// Situacao do pedido. Sao apenas tres porque sao as tres que o sistema sabe
/// tratar hoje -- estado que ninguem transiciona e complexidade sem uso.
/// </summary>
public enum OrderStatus
{
    /// <summary>Em edicao. E a unica situacao em que os itens podem mudar.</summary>
    Draft = 1,

    /// <summary>Confirmado pelo cliente. Nao aceita mais alteracao de itens.</summary>
    Placed = 2,

    /// <summary>Cancelado. Situacao final.</summary>
    Cancelled = 3,
}
