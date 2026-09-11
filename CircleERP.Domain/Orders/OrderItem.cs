using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Shared;

namespace CircleERP.Domain.Orders;

/// <summary>
/// Linha de um pedido.
/// </summary>
/// <remarks>
/// Entidade interna do agregado: so existe dentro de um <see cref="Order"/> e
/// so e criada e alterada por ele. Por isso nao ha repositorio de itens -- quem
/// carrega e salva a linha e sempre a raiz.
/// </remarks>
public sealed class OrderItem : Entity<int>
{
    /// <summary>Exigido pelo EF Core.</summary>
    private OrderItem()
    {
        Description = null!;
        Quantity = null!;
        UnitPrice = null!;
    }

    internal OrderItem(
        int productId,
        ItemDescription description,
        Quantity quantity,
        Money unitPrice)
    {
        ProductId = productId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int OrderId { get; private set; }

    /// <summary>Produto vendido, referenciado por identidade.</summary>
    public int ProductId { get; private set; }

    /// <summary>
    /// Nome do produto no momento da venda.
    /// </summary>
    /// <remarks>
    /// E uma copia, e nao uma leitura do cadastro: renomear um produto nao pode
    /// reescrever o que um pedido antigo diz ter vendido. Mesmo raciocinio do
    /// <see cref="UnitPrice"/>.
    /// </remarks>
    public ItemDescription Description { get; private set; }

    public Quantity Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    /// <summary>Valor da linha. Calculado, nunca informado de fora.</summary>
    public Money LineTotal => UnitPrice.Multiply(Quantity.Value);

    internal void ChangeQuantity(Quantity quantity) => Quantity = quantity;
}
