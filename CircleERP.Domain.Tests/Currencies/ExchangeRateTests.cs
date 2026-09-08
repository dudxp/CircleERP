using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;

namespace CircleERP.Domain.Tests.Currencies;

[TestFixture]
public class ExchangeRateTests
{
    [Test]
    public void Aceita_taxa_positiva()
    {
        var rate = ExchangeRate.Create(5.4321m);

        Assert.That(rate.Value, Is.EqualTo(5.4321m));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-0.000001)]
    public void Recusa_taxa_nao_positiva(decimal input)
    {
        Assert.That(() => ExchangeRate.Create(input), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_precisao_alem_do_que_a_coluna_guarda()
    {
        // Sete casas decimais: a coluna guarda seis, entao aceitar aqui seria
        // gravar um valor silenciosamente diferente do informado.
        Assert.That(() => ExchangeRate.Create(1.1234567m), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Preserva_a_precisao_decimal_exata()
    {
        // O motivo de nao usar float: 0.1 + 0.2 != 0.3 em ponto flutuante binario.
        var rate = ExchangeRate.Create(0.1m + 0.2m);

        Assert.That(rate.Value, Is.EqualTo(0.3m));
    }
}
