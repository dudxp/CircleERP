using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Addresses;

namespace CircleERP.Domain.Tests.Addresses;

[TestFixture]
public class AddressTests
{
    private static Address Register(string? complement = null) =>
        Address.Register(
            ZipCode.Create("01310-100"),
            AddressText.Create("Avenida Paulista", "Logradouro", Address.StreetMaxLength),
            AddressText.Create("1578", "Numero", Address.NumberMaxLength),
            AddressText.CreateOrNull(complement, "Complemento", Address.ComplementMaxLength),
            AddressText.Create("Bela Vista", "Bairro", Address.DistrictMaxLength),
            AddressText.Create("Sao Paulo", "Cidade", Address.CityMaxLength),
            StateCode.Create("sp"));

    [Test]
    public void Cep_guarda_apenas_digitos()
    {
        Assert.That(Register().ZipCode.Value, Is.EqualTo("01310100"));
    }

    [Test]
    public void Uf_e_normalizada_para_maiusculas()
    {
        Assert.That(Register().State.Value, Is.EqualTo("SP"));
    }

    [Test]
    public void Complemento_e_opcional()
    {
        Assert.That(Register().Complement, Is.Null);
    }

    [Test]
    public void Endereco_em_uma_linha_omite_complemento_ausente()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                Register().ToSingleLine(),
                Is.EqualTo("Avenida Paulista, 1578 - Bela Vista, Sao Paulo/SP"));
            Assert.That(
                Register("Apto 42").ToSingleLine(),
                Is.EqualTo("Avenida Paulista, 1578 - Apto 42 - Bela Vista, Sao Paulo/SP"));
        });
    }

    [TestCase("0131010")]
    [TestCase("013101000")]
    [TestCase("")]
    [TestCase(null)]
    public void Recusa_cep_fora_de_oito_digitos(string? input)
    {
        Assert.That(() => ZipCode.Create(input), Throws.TypeOf<DomainException>());
    }

    [TestCase("XX")]
    [TestCase("SPP")]
    [TestCase("")]
    [TestCase(null)]
    public void Recusa_uf_que_nao_existe(string? input)
    {
        // "XX" tem o formato certo e mesmo assim nao e um estado.
        Assert.That(() => StateCode.Create(input), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Existem_vinte_e_sete_unidades_federativas()
    {
        Assert.That(StateCode.All, Has.Count.EqualTo(27));
    }

    [Test]
    public void Numero_aceita_texto()
    {
        // "S/N" e "123A" sao numeros de endereco validos.
        Assert.That(
            AddressText.Create("S/N", "Numero", Address.NumberMaxLength).Value,
            Is.EqualTo("S/N"));
    }

    [Test]
    public void Recusa_campo_obrigatorio_vazio()
    {
        Assert.That(
            () => AddressText.Create("  ", "Logradouro", Address.StreetMaxLength),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_campo_alem_do_limite()
    {
        var tooLong = new string('a', Address.StreetMaxLength + 1);

        Assert.That(
            () => AddressText.Create(tooLong, "Logradouro", Address.StreetMaxLength),
            Throws.TypeOf<DomainException>());
    }
}
