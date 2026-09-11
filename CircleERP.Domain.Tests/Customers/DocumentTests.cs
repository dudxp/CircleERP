using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Customers;

namespace CircleERP.Domain.Tests.Customers;

[TestFixture]
public class DocumentTests
{
    // CPFs e CNPJs sinteticos, validos pelo calculo dos digitos verificadores.
    private const string ValidCpf = "52998224725";
    private const string ValidCnpj = "11222333000181";

    [Test]
    public void Aceita_cpf_valido_para_pessoa_fisica()
    {
        var document = Document.Create(ValidCpf, CustomerType.Individual);

        Assert.Multiple(() =>
        {
            Assert.That(document.Value, Is.EqualTo(ValidCpf));
            Assert.That(document.Type, Is.EqualTo(CustomerType.Individual));
        });
    }

    [Test]
    public void Aceita_cnpj_valido_para_pessoa_juridica()
    {
        var document = Document.Create(ValidCnpj, CustomerType.Company);

        Assert.That(document.Type, Is.EqualTo(CustomerType.Company));
    }

    [TestCase("529.982.247-25", ValidCpf)]
    [TestCase("11.222.333/0001-81", ValidCnpj)]
    [TestCase("  52998224725  ", ValidCpf)]
    public void Guarda_apenas_digitos(string input, string expected)
    {
        var type = expected.Length == 11 ? CustomerType.Individual : CustomerType.Company;

        Assert.That(Document.Create(input, type).Value, Is.EqualTo(expected));
    }

    [Test]
    public void Documento_com_e_sem_pontuacao_e_o_mesmo_documento()
    {
        // Sem isso, o mesmo cliente entraria duas vezes no cadastro.
        Assert.That(
            Document.Create("529.982.247-25", CustomerType.Individual),
            Is.EqualTo(Document.Create("52998224725", CustomerType.Individual)));
    }

    [Test]
    public void Recusa_cpf_com_digito_verificador_errado()
    {
        Assert.That(
            () => Document.Create("52998224726", CustomerType.Individual),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_cnpj_com_digito_verificador_errado()
    {
        Assert.That(
            () => Document.Create("11222333000182", CustomerType.Company),
            Throws.TypeOf<DomainException>());
    }

    [TestCase("00000000000")]
    [TestCase("11111111111")]
    public void Recusa_cpf_de_digitos_repetidos(string input)
    {
        // Passam no calculo dos digitos verificadores, mas nao sao CPFs.
        Assert.That(
            () => Document.Create(input, CustomerType.Individual),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_cnpj_de_digitos_repetidos()
    {
        Assert.That(
            () => Document.Create("11111111111111", CustomerType.Company),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_cpf_declarado_como_pessoa_juridica()
    {
        // O documento tem de corresponder ao tipo: 11 digitos nao e CNPJ.
        Assert.That(
            () => Document.Create(ValidCpf, CustomerType.Company),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Recusa_cnpj_declarado_como_pessoa_fisica()
    {
        Assert.That(
            () => Document.Create(ValidCnpj, CustomerType.Individual),
            Throws.TypeOf<DomainException>());
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("abc")]
    public void Recusa_documento_sem_digitos(string? input)
    {
        Assert.That(
            () => Document.Create(input, CustomerType.Individual),
            Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Formata_para_exibicao()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                Document.Create(ValidCpf, CustomerType.Individual).ToFormattedString(),
                Is.EqualTo("529.982.247-25"));
            Assert.That(
                Document.Create(ValidCnpj, CustomerType.Company).ToFormattedString(),
                Is.EqualTo("11.222.333/0001-81"));
        });
    }
}
