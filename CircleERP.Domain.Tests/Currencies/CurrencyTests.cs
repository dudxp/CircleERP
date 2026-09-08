using CircleERP.Domain.Currencies;
using CircleERP.Domain.Currencies.Events;

namespace CircleERP.Domain.Tests.Currencies;

[TestFixture]
public class CurrencyTests
{
    private static Currency Register(
        string code = "BRL",
        decimal rate = 1m,
        string? symbol = null) =>
        Currency.Register(
            CurrencyCode.Create(code),
            CurrencyDescription.Create("Real brasileiro"),
            ExchangeRate.Create(rate),
            CurrencySymbol.CreateOrNull(symbol));

    [Test]
    public void Registrar_deixa_a_moeda_com_os_dados_informados()
    {
        var currency = Register("usd", 5.25m);

        Assert.Multiple(() =>
        {
            Assert.That(currency.Code.Value, Is.EqualTo("USD"));
            Assert.That(currency.Description.Value, Is.EqualTo("Real brasileiro"));
            Assert.That(currency.Rate.Value, Is.EqualTo(5.25m));
        });
    }

    [Test]
    public void Registrar_publica_evento_de_moeda_cadastrada()
    {
        var currency = Register("EUR");

        var registered = currency.DomainEvents.OfType<CurrencyRegistered>().Single();

        Assert.That(registered.Code, Is.EqualTo("EUR"));
    }

    [Test]
    public void Alterar_a_taxa_publica_evento_com_o_valor_anterior_e_o_novo()
    {
        var currency = Register(rate: 1m);
        currency.ClearDomainEvents();

        currency.ChangeRate(ExchangeRate.Create(2m));

        var changed = currency.DomainEvents.OfType<CurrencyRateChanged>().Single();

        Assert.Multiple(() =>
        {
            Assert.That(changed.PreviousRate, Is.EqualTo(1m));
            Assert.That(changed.CurrentRate, Is.EqualTo(2m));
            Assert.That(currency.Rate.Value, Is.EqualTo(2m));
        });
    }

    [Test]
    public void Alterar_a_taxa_para_o_mesmo_valor_nao_publica_evento()
    {
        var currency = Register(rate: 3m);
        currency.ClearDomainEvents();

        currency.ChangeRate(ExchangeRate.Create(3m));

        Assert.That(currency.DomainEvents, Is.Empty);
    }

    [Test]
    public void Moeda_pode_existir_sem_simbolo()
    {
        Assert.That(Register().Symbol, Is.Null);
    }

    [Test]
    public void Codigo_e_simbolo_sao_campos_independentes()
    {
        // O caso real do banco: "R$" e simbolo, "BRL" e o codigo ISO.
        var currency = Register("BRL", symbol: "R$");

        Assert.Multiple(() =>
        {
            Assert.That(currency.Code.Value, Is.EqualTo("BRL"));
            Assert.That(currency.Symbol!.Value, Is.EqualTo("R$"));
        });
    }

    [Test]
    public void Alterar_o_simbolo_para_vazio_remove_o_simbolo()
    {
        var currency = Register("BRL", symbol: "R$");

        currency.ChangeSymbol(CurrencySymbol.CreateOrNull(null));

        Assert.That(currency.Symbol, Is.Null);
    }

    [Test]
    public void Alterar_a_descricao_nao_mexe_na_taxa()
    {
        var currency = Register(rate: 4m);

        currency.ChangeDescription(CurrencyDescription.Create("Dolar americano"));

        Assert.Multiple(() =>
        {
            Assert.That(currency.Description.Value, Is.EqualTo("Dolar americano"));
            Assert.That(currency.Rate.Value, Is.EqualTo(4m));
        });
    }
}
