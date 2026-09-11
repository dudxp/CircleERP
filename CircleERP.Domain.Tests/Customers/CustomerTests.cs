using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Customers;
using CircleERP.Domain.Customers.Events;

namespace CircleERP.Domain.Tests.Customers;

[TestFixture]
public class CustomerTests
{
    private const string ValidCpf = "52998224725";
    private const string ValidCnpj = "11222333000181";
    private static readonly DateTime Now = new(2026, 9, 11, 12, 0, 0, DateTimeKind.Utc);

    private static Customer Register(string document = ValidCpf, int? addressId = null)
    {
        var type = document.Length == 11 ? CustomerType.Individual : CustomerType.Company;

        return Customer.Register(
            CustomerName.Create("Eduardo"),
            Document.Create(document, type),
            addressId);
    }

    [Test]
    public void Cliente_nasce_ativo()
    {
        Assert.That(Register().IsActive, Is.True);
    }

    [Test]
    public void Tipo_vem_do_documento()
    {
        // Nao ha como marcar como pessoa juridica portando um CPF: o tipo nao e
        // um campo independente.
        Assert.Multiple(() =>
        {
            Assert.That(Register(ValidCpf).Type, Is.EqualTo(CustomerType.Individual));
            Assert.That(Register(ValidCnpj).Type, Is.EqualTo(CustomerType.Company));
        });
    }

    [Test]
    public void Registrar_publica_evento_com_o_documento()
    {
        var registered = Register().DomainEvents.OfType<CustomerRegistered>().Single();

        Assert.That(registered.Document, Is.EqualTo(ValidCpf));
    }

    [Test]
    public void Cliente_pode_existir_sem_endereco()
    {
        Assert.That(Register().AddressId, Is.Null);
    }

    [Test]
    public void Vincular_e_desvincular_endereco()
    {
        var customer = Register();

        customer.LinkAddress(7);
        Assert.That(customer.AddressId, Is.EqualTo(7));

        customer.UnlinkAddress();
        Assert.That(customer.AddressId, Is.Null);
    }

    [Test]
    public void Trocar_documento_troca_o_tipo_junto()
    {
        var customer = Register(ValidCpf);

        customer.Change(
            CustomerName.Create("Empresa LTDA"),
            Document.Create(ValidCnpj, CustomerType.Company));

        Assert.Multiple(() =>
        {
            Assert.That(customer.Type, Is.EqualTo(CustomerType.Company));
            Assert.That(customer.Name.Value, Is.EqualTo("Empresa LTDA"));
        });
    }

    [Test]
    public void Inativar_publica_evento_e_inativar_de_novo_e_recusado()
    {
        var customer = Register();
        customer.ClearDomainEvents();

        customer.Deactivate(Now);

        Assert.Multiple(() =>
        {
            Assert.That(customer.IsActive, Is.False);
            Assert.That(customer.DomainEvents.OfType<CustomerDeactivated>().Count(), Is.EqualTo(1));
            Assert.That(() => customer.Deactivate(Now), Throws.TypeOf<DomainException>());
        });
    }

    [Test]
    public void Reativar_cliente_ativo_e_recusado()
    {
        var customer = Register();

        Assert.That(() => customer.Activate(), Throws.TypeOf<DomainException>());
    }

    [Test]
    public void Inativar_e_reativar_devolve_o_cliente_ao_ar()
    {
        var customer = Register();

        customer.Deactivate(Now);
        customer.Activate();

        Assert.That(customer.IsActive, Is.True);
    }
}
