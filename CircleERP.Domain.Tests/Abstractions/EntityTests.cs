using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Tests.Abstractions;

[TestFixture]
public class EntityTests
{
    private sealed class Customer : Entity<int>
    {
        public Customer(int id, string name) : base(id) => Name = name;

        public string Name { get; }
    }

    private sealed class Supplier(int id) : Entity<int>(id);

    private sealed record CustomerRegistered(DateTime OccurredOnUtc) : IDomainEvent;

    private sealed class Auditable : Entity<int>
    {
        public Auditable(int id) : base(id) { }

        public void Register() => Raise(new CustomerRegistered(DateTime.UtcNow));
    }

    [Test]
    public void Entidades_com_o_mesmo_id_sao_iguais_ainda_que_os_atributos_difiram()
    {
        var first = new Customer(1, "Eduardo");
        var second = new Customer(1, "Outro nome");

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.EqualTo(second));
            Assert.That(first == second, Is.True);
        });
    }

    [Test]
    public void Entidades_com_ids_diferentes_nao_sao_iguais()
    {
        var first = new Customer(1, "Eduardo");
        var second = new Customer(2, "Eduardo");

        Assert.That(first, Is.Not.EqualTo(second));
    }

    [Test]
    public void Tipos_diferentes_com_o_mesmo_id_nao_sao_iguais()
    {
        Entity<int> customer = new Customer(1, "Eduardo");
        Entity<int> supplier = new Supplier(1);

        Assert.That(customer, Is.Not.EqualTo(supplier));
    }

    [Test]
    public void Entidades_ainda_nao_persistidas_nao_sao_iguais_entre_si()
    {
        var first = new Customer(0, "Eduardo");
        var second = new Customer(0, "Eduardo");

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.Not.EqualTo(second));
            Assert.That(first, Is.EqualTo(first));
        });
    }

    [Test]
    public void Eventos_de_dominio_ficam_acumulados_ate_serem_limpos()
    {
        var auditable = new Auditable(1);

        auditable.Register();
        auditable.Register();

        Assert.That(auditable.DomainEvents, Has.Count.EqualTo(2));

        auditable.ClearDomainEvents();

        Assert.That(auditable.DomainEvents, Is.Empty);
    }
}
