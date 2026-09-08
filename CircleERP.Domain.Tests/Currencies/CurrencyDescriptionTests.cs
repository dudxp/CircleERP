using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;

namespace CircleERP.Domain.Tests.Currencies;

[TestFixture]
public class CurrencyDescriptionTests
{
    [Test]
    public void Remove_espacos_das_pontas()
    {
        Assert.That(CurrencyDescription.Create("  Real brasileiro  ").Value,
            Is.EqualTo("Real brasileiro"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Recusa_descricao_vazia(string? input)
    {
        Assert.That(() => CurrencyDescription.Create(input), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_descricao_maior_que_o_limite_da_coluna()
    {
        var tooLong = new string('a', CurrencyDescription.MaxLength + 1);

        Assert.That(() => CurrencyDescription.Create(tooLong), Throws.TypeOf<DomainException>());
    }
}
