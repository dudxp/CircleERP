using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;

namespace CircleERP.Domain.Tests.Currencies;

[TestFixture]
public class CurrencyCodeTests
{
    [TestCase("brl", "BRL")]
    [TestCase("  usd  ", "USD")]
    [TestCase("EUR", "EUR")]
    public void Normaliza_para_maiusculas_sem_espacos(string input, string expected)
    {
        var code = CurrencyCode.Create(input);

        Assert.That(code.Value, Is.EqualTo(expected));
    }

    [Test]
    public void Codigos_equivalentes_em_caixas_diferentes_sao_iguais()
    {
        Assert.That(CurrencyCode.Create("brl"), Is.EqualTo(CurrencyCode.Create("BRL")));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Recusa_codigo_vazio(string? input)
    {
        Assert.That(() => CurrencyCode.Create(input), Throws.TypeOf<DomainException>());
    }

    [TestCase("BR")]
    [TestCase("BRLL")]
    public void Recusa_codigo_fora_das_tres_letras(string input)
    {
        Assert.That(() => CurrencyCode.Create(input), Throws.TypeOf<DomainException>());
    }

    [TestCase("BR1")]
    [TestCase("B$L")]
    public void Recusa_codigo_com_caractere_nao_alfabetico(string input)
    {
        Assert.That(() => CurrencyCode.Create(input), Throws.TypeOf<DomainException>());
    }
}
