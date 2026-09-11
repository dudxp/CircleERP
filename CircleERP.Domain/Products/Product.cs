using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Products.Events;
using CircleERP.Domain.Shared;

namespace CircleERP.Domain.Products;

/// <summary>
/// Produto vendavel. Raiz de agregado.
/// </summary>
/// <remarks>
/// O preco e um <see cref="Money"/>, ou seja, carrega a moeda junto. Um produto
/// cotado em BRL nao preenche sozinho a linha de um pedido em USD -- a conversao
/// e uma decisao comercial, nao uma multiplicacao escondida.
///
/// Reajustar este preco nao mexe em pedido nenhum: a linha do pedido guarda o
/// preco praticado na venda.
/// </remarks>
public sealed class Product : Entity<int>, IAggregateRoot
{
    /// <summary>Exigido pelo EF Core.</summary>
    private Product()
    {
        Sku = null!;
        Name = null!;
        Price = null!;
    }

    private Product(Sku sku, ProductName name, Money price, UnitOfMeasure unit)
    {
        Sku = sku;
        Name = name;
        Price = price;
        Unit = unit;
        IsActive = true;
    }

    public Sku Sku { get; private set; }

    public ProductName Name { get; private set; }

    /// <summary>Preco de venda padrao. Sugere a linha do pedido, nao a impoe.</summary>
    public Money Price { get; private set; }

    public UnitOfMeasure Unit { get; private set; }

    /// <summary>
    /// Produto inativo nao entra em pedidos novos, mas continua existindo: as
    /// linhas que ja o venderam precisam seguir legiveis.
    /// </summary>
    public bool IsActive { get; private set; }

    public static Product Register(Sku sku, ProductName name, Money price, UnitOfMeasure unit)
    {
        var product = new Product(sku, name, price, unit);

        product.Raise(new ProductRegistered(sku.Value, DateTime.UtcNow));

        return product;
    }

    public void Change(Sku sku, ProductName name, Money price, UnitOfMeasure unit)
    {
        Sku = sku;
        Name = name;
        Price = price;
        Unit = unit;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("O produto ja esta inativo.");

        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("O produto ja esta ativo.");

        IsActive = true;
    }
}
