using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;

namespace CircleERP.Domain.Tests.Currencies;

[TestFixture]
public class CurrencySymbolTests
{
    [TestCase("R$")]
    [TestCase("US$")]
    [TestCase("EUR")]
    public void Aceita_simbolos_que_nao_sao_codigos_iso(string input)
    {
        // O simbolo existe justamente para o que o codigo ISO nao permite.
        Assert.That(CurrencySymbol.CreateOrNull(input)!.Value, Is.EqualTo(input));
    }

    [Test]
    public void Remove_espacos_das_pontas()
    {
        Assert.That(CurrencySymbol.CreateOrNull("  R$  ")!.Value, Is.EqualTo("R$"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Ausencia_de_simbolo_e_valida_e_devolve_null(string? input)
    {
        Assert.That(CurrencySymbol.CreateOrNull(input), Is.Null);
    }

    [Test]
    public void Recusa_simbolo_maior_que_o_limite_da_coluna()
    {
        var tooLong = new string('$', CurrencySymbol.MaxLength + 1);

        Assert.That(() => CurrencySymbol.CreateOrNull(tooLong), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Simbolos_iguais_sao_iguais()
    {
        Assert.That(CurrencySymbol.CreateOrNull("R$"), Is.EqualTo(CurrencySymbol.CreateOrNull("R$")));
    }
}
