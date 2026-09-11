using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Orders.Events;
using CircleERP.Domain.Shared;

namespace CircleERP.Domain.Orders;

/// <summary>
/// Pedido de venda. Raiz de agregado: os itens so existem atraves dele, e
/// nenhuma alteracao de item acontece sem passar por aqui.
/// </summary>
/// <remarks>
/// Tanto o cliente quanto a moeda sao referenciados por identidade, e nao por
/// objeto: agregados nunca carregam outros agregados por dentro. Se o pedido
/// guardasse a moeda inteira, um pedido antigo passaria a valer pela taxa de
/// hoje; se guardasse o cliente, renomear o cliente reescreveria o historico.
///
/// A regra "o cliente precisa existir e estar ativo" e uma regra *entre*
/// agregados, e por isso vive no caso de uso, nao aqui.
/// </remarks>
public sealed class Order : Entity<int>, IAggregateRoot
{
    private readonly List<OrderItem> _items = [];

    /// <summary>Exigido pelo EF Core.</summary>
    private Order()
    {
        Currency = null!;
    }

    private Order(int customerId, CurrencyCode currency, DateTime createdOnUtc)
    {
        CustomerId = customerId;
        Currency = currency;
        CreatedOnUtc = createdOnUtc;
        Status = OrderStatus.Draft;
    }

    /// <summary>Cliente do pedido, referenciado por identidade.</summary>
    public int CustomerId { get; private set; }

    /// <summary>Moeda em que o pedido inteiro e expresso.</summary>
    public CurrencyCode Currency { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime? PlacedOnUtc { get; private set; }

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Soma das linhas. E uma propriedade calculada de proposito: total gravado
    /// e total que pode divergir dos itens.
    /// </summary>
    public Money Total =>
        _items.Aggregate(Money.Zero(Currency), (total, item) => total.Add(item.LineTotal));

    /// <summary>Abre um pedido em rascunho, sem itens.</summary>
    public static Order Open(int customerId, CurrencyCode currency, DateTime createdOnUtc) =>
        new(customerId, currency, createdOnUtc);

    /// <summary>
    /// Adiciona uma linha.
    /// </summary>
    /// <remarks>
    /// O preco unitario e construido na moeda do pedido, e nao recebido pronto
    /// -- assim nao existe caminho para uma linha em moeda diferente da do
    /// pedido.
    ///
    /// A descricao chega pronta em vez de ser lida do produto porque o agregado
    /// nao alcanca outro agregado. Quem copia o nome do produto e o caso de uso,
    /// no instante da venda.
    /// </remarks>
    public OrderItem AddItem(
        int productId,
        ItemDescription description,
        Quantity quantity,
        decimal unitPrice)
    {
        EnsureIsDraft("adicionar itens");

        var item = new OrderItem(productId, description, quantity, Money.Create(unitPrice, Currency));
        _items.Add(item);

        return item;
    }

    public void ChangeItemQuantity(int itemId, Quantity quantity)
    {
        EnsureIsDraft("alterar itens");

        FindItem(itemId).ChangeQuantity(quantity);
    }

    public void RemoveItem(int itemId)
    {
        EnsureIsDraft("remover itens");

        _items.Remove(FindItem(itemId));
    }

    /// <summary>Confirma o pedido.</summary>
    public void Place(DateTime placedOnUtc)
    {
        EnsureIsDraft("confirmar o pedido");

        if (_items.Count == 0)
            throw new DomainException("Nao e possivel confirmar um pedido sem itens.");

        Status = OrderStatus.Placed;
        PlacedOnUtc = placedOnUtc;

        Raise(new OrderPlaced(Id, CustomerId, Currency.Value, Total.Amount, placedOnUtc));
    }

    public void Cancel(DateTime cancelledOnUtc)
    {
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("O pedido ja esta cancelado.");

        var previous = Status;
        Status = OrderStatus.Cancelled;

        Raise(new OrderCancelled(Id, previous, cancelledOnUtc));
    }

    /// <remarks>
    /// O id do item vem do banco, entao so existe depois que o pedido e salvo.
    /// Enderecar item por id pressupoe um item ja persistido -- o que o fluxo da
    /// API garante, porque cada requisicao confirma a unidade de trabalho.
    /// </remarks>
    private OrderItem FindItem(int itemId) =>
        _items.FirstOrDefault(item => item.Id == itemId)
        ?? throw new DomainException($"Item {itemId} nao pertence a este pedido.");

    private void EnsureIsDraft(string action)
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException(
                $"Nao e possivel {action}: o pedido esta como {Status} e so rascunhos podem ser alterados.");
    }
}
