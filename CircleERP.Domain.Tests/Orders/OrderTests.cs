using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Orders;
using CircleERP.Domain.Orders.Events;

namespace CircleERP.Domain.Tests.Orders;

[TestFixture]
public class OrderTests
{
    private static readonly DateTime Now = new(2026, 9, 8, 12, 0, 0, DateTimeKind.Utc);

    private static Order Open(string currency = "BRL") =>
        Order.Open(
            CustomerName.Create("Eduardo"),
            CurrencyCode.Create(currency),
            Now);

    private static OrderItem AddItem(Order order, int quantity = 1, decimal unitPrice = 10m) =>
        order.AddItem(ItemDescription.Create("Teclado"), Quantity.Create(quantity), unitPrice);

    [Test]
    public void Pedido_nasce_em_rascunho_sem_itens_e_com_total_zero()
    {
        var order = Open();

        Assert.Multiple(() =>
        {
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Draft));
            Assert.That(order.Items, Is.Empty);
            Assert.That(order.Total.Amount, Is.Zero);
            Assert.That(order.Total.Currency, Is.EqualTo(order.Currency));
            Assert.That(order.PlacedOnUtc, Is.Null);
        });
    }

    [Test]
    public void Total_e_a_soma_das_linhas()
    {
        var order = Open();

        AddItem(order, quantity: 3, unitPrice: 10.50m);
        AddItem(order, quantity: 2, unitPrice: 4.25m);

        Assert.That(order.Total.Amount, Is.EqualTo(40.00m));
    }

    [Test]
    public void Item_herda_a_moeda_do_pedido()
    {
        var order = Open("USD");

        var item = AddItem(order);

        // Nao existe caminho para uma linha em moeda diferente da do pedido:
        // o preco unitario e construido a partir da moeda da raiz.
        Assert.That(item.UnitPrice.Currency, Is.EqualTo(order.Currency));
    }

    [Test]
    public void Total_da_linha_e_preco_vezes_quantidade()
    {
        var order = Open();

        var item = AddItem(order, quantity: 4, unitPrice: 2.50m);

        Assert.That(item.LineTotal.Amount, Is.EqualTo(10.00m));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Quantidade_nao_positiva_e_recusada(int quantity)
    {
        var order = Open();

        Assert.That(
            () => AddItem(order, quantity: quantity),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Preco_unitario_negativo_e_recusado()
    {
        var order = Open();

        Assert.That(
            () => AddItem(order, unitPrice: -1m),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Confirmar_pedido_sem_itens_e_recusado()
    {
        var order = Open();

        Assert.That(() => order.Place(Now), Throws.TypeOf<DomainException>());
        Assert.That(order.Status, Is.EqualTo(OrderStatus.Draft), "a situacao nao muda quando a confirmacao falha");
    }

    [Test]
    public void Confirmar_muda_a_situacao_registra_a_data_e_publica_evento()
    {
        var order = Open();
        AddItem(order, quantity: 2, unitPrice: 7.50m);

        order.Place(Now);

        var placed = order.DomainEvents.OfType<OrderPlaced>().Single();

        Assert.Multiple(() =>
        {
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Placed));
            Assert.That(order.PlacedOnUtc, Is.EqualTo(Now));
            Assert.That(placed.Total, Is.EqualTo(15.00m));
            Assert.That(placed.Currency, Is.EqualTo("BRL"));
        });
    }

    [Test]
    public void Pedido_confirmado_nao_aceita_mais_itens()
    {
        var order = Open();
        AddItem(order);
        order.Place(Now);

        Assert.That(() => AddItem(order), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Pedido_confirmado_nao_aceita_remocao_nem_alteracao_de_item()
    {
        var order = Open();
        var item = AddItem(order);
        order.Place(Now);

        Assert.Multiple(() =>
        {
            Assert.That(() => order.RemoveItem(item.Id), Throws.TypeOf<DomainException>());
            Assert.That(
                () => order.ChangeItemQuantity(item.Id, Quantity.Create(5)),
                Throws.TypeOf<DomainException>());
        });
    }

    [Test]
    public void Confirmar_duas_vezes_e_recusado()
    {
        var order = Open();
        AddItem(order);
        order.Place(Now);

        Assert.That(() => order.Place(Now), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Cancelar_publica_evento_com_a_situacao_anterior()
    {
        var order = Open();
        AddItem(order);
        order.Place(Now);
        order.ClearDomainEvents();

        order.Cancel(Now);

        var cancelled = order.DomainEvents.OfType<OrderCancelled>().Single();

        Assert.Multiple(() =>
        {
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Cancelled));
            Assert.That(cancelled.PreviousStatus, Is.EqualTo(OrderStatus.Placed));
        });
    }

    [Test]
    public void Cancelar_duas_vezes_e_recusado()
    {
        var order = Open();
        order.Cancel(Now);

        Assert.That(() => order.Cancel(Now), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Pedido_cancelado_nao_aceita_itens()
    {
        var order = Open();
        order.Cancel(Now);

        Assert.That(() => AddItem(order), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Operar_sobre_item_de_outro_pedido_e_recusado()
    {
        var order = Open();
        AddItem(order);

        Assert.That(() => order.RemoveItem(9999), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void A_colecao_de_itens_e_somente_leitura_de_fora()
    {
        var order = Open();

        // Items expoe IReadOnlyList: nao ha Add publico que escape das regras.
        Assert.That(order.Items, Is.InstanceOf<IReadOnlyList<OrderItem>>());
        Assert.That(order.Items, Is.Not.InstanceOf<List<OrderItem>>());
    }
}
