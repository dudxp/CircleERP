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

    internal OrderItem(ItemDescription description, Quantity quantity, Money unitPrice)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int OrderId { get; private set; }

    public ItemDescription Description { get; private set; }

    public Quantity Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    /// <summary>Valor da linha. Calculado, nunca informado de fora.</summary>
    public Money LineTotal => UnitPrice.Multiply(Quantity.Value);

    internal void ChangeQuantity(Quantity quantity) => Quantity = quantity;
}
