using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Shared;

namespace CircleERP.Domain.Tests.Shared;

[TestFixture]
public class MoneyTests
{
    private static readonly CurrencyCode Brl = CurrencyCode.Create("BRL");
    private static readonly CurrencyCode Usd = CurrencyCode.Create("USD");

    [Test]
    public void Somar_valores_na_mesma_moeda_soma_as_quantias()
    {
        var total = Money.Create(10.50m, Brl).Add(Money.Create(4.50m, Brl));

        Assert.That(total.Amount, Is.EqualTo(15.00m));
    }

    [Test]
    public void Somar_moedas_diferentes_e_recusado()
    {
        // O motivo de existir o Money: guardar so o numero deixaria somar
        // reais com dolares sem ninguem perceber.
        var brl = Money.Create(10m, Brl);
        var usd = Money.Create(10m, Usd);

        Assert.That(() => brl.Add(usd), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Multiplicar_mantem_a_moeda()
    {
        var line = Money.Create(2.50m, Brl).Multiply(4);

        Assert.Multiple(() =>
        {
            Assert.That(line.Amount, Is.EqualTo(10.00m));
            Assert.That(line.Currency, Is.EqualTo(Brl));
        });
    }

    [Test]
    public void Recusa_quantia_negativa()
    {
        Assert.That(() => Money.Create(-0.01m, Brl), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_mais_de_duas_casas_decimais()
    {
        Assert.That(() => Money.Create(1.234m, Brl), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Mesma_quantia_em_moedas_diferentes_nao_sao_iguais()
    {
        Assert.That(Money.Create(10m, Brl), Is.Not.EqualTo(Money.Create(10m, Usd)));
    }
}
