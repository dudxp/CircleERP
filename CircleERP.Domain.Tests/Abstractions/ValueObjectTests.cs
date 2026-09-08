using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Tests.Abstractions;

[TestFixture]
public class ValueObjectTests
{
    private sealed class Coordinate(int x, int y) : ValueObject
    {
        public int X { get; } = x;
        public int Y { get; } = y;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }

    private sealed class Point(int x, int y) : ValueObject
    {
        public int X { get; } = x;
        public int Y { get; } = y;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }

    [Test]
    public void Instancias_com_os_mesmos_atributos_sao_iguais()
    {
        var first = new Coordinate(1, 2);
        var second = new Coordinate(1, 2);

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.EqualTo(second));
            Assert.That(first == second, Is.True);
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        });
    }

    [Test]
    public void Instancias_com_atributos_diferentes_nao_sao_iguais()
    {
        var first = new Coordinate(1, 2);
        var second = new Coordinate(2, 1);

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.Not.EqualTo(second));
            Assert.That(first != second, Is.True);
        });
    }

    [Test]
    public void Tipos_diferentes_com_os_mesmos_atributos_nao_sao_iguais()
    {
        ValueObject coordinate = new Coordinate(1, 2);
        ValueObject point = new Point(1, 2);

        Assert.That(coordinate, Is.Not.EqualTo(point));
    }

    [Test]
    public void Comparacao_com_null_nao_lanca_e_retorna_falso()
    {
        var coordinate = new Coordinate(1, 2);

        Assert.Multiple(() =>
        {
            Assert.That(coordinate.Equals(null), Is.False);
            Assert.That(coordinate == null, Is.False);
            Assert.That(coordinate != null, Is.True);
        });
    }
}
