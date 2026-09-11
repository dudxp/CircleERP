using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Products;
using CircleERP.Domain.Products.Events;
using CircleERP.Domain.Shared;

namespace CircleERP.Domain.Tests.Products;

[TestFixture]
public class ProductTests
{
    private static readonly CurrencyCode Brl = CurrencyCode.Create("BRL");

    private static Product Register(string sku = "TEC-001", decimal price = 10.00m) =>
        Product.Register(
            Sku.Create(sku),
            ProductName.Create("Teclado mecânico"),
            Money.Create(price, Brl),
            UnitOfMeasure.Unit);

    [Test]
    public void Produto_nasce_ativo()
    {
        Assert.That(Register().IsActive, Is.True);
    }

    [Test]
    public void Registrar_publica_evento_com_o_sku()
    {
        var registered = Register().DomainEvents.OfType<ProductRegistered>().Single();

        Assert.That(registered.Sku, Is.EqualTo("TEC-001"));
    }

    [Test]
    public void Preco_carrega_a_moeda_junto()
    {
        // Um produto cotado em BRL nao e um numero solto: e 10 reais.
        var product = Register(price: 10m);

        Assert.Multiple(() =>
        {
            Assert.That(product.Price.Amount, Is.EqualTo(10m));
            Assert.That(product.Price.Currency, Is.EqualTo(Brl));
        });
    }

    [TestCase("tec-001", "TEC-001")]
    [TestCase("  abc  ", "ABC")]
    public void Sku_e_normalizado_para_maiusculas(string input, string expected)
    {
        Assert.That(Sku.Create(input).Value, Is.EqualTo(expected));
    }

    [Test]
    public void Sku_recusa_espaco_no_meio()
    {
        // Espaco invisivel no codigo e fonte classica de duplicata.
        Assert.That(() => Sku.Create("TEC 001"), Throws.TypeOf<DomainException>());
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Sku_recusa_vazio(string? input)
    {
        Assert.That(() => Sku.Create(input), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Preco_negativo_e_recusado()
    {
        Assert.That(() => Register(price: -1m), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Inativar_e_reativar()
    {
        var product = Register();

        product.Deactivate();
        Assert.That(product.IsActive, Is.False);

        product.Activate();
        Assert.That(product.IsActive, Is.True);
    }

    [Test]
    public void Inativar_duas_vezes_e_recusado()
    {
        var product = Register();
        product.Deactivate();

        Assert.That(() => product.Deactivate(), Throws.TypeOf<DomainException>());
    }
}
