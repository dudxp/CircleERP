using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Orders;

/// <summary>Quantidade de um item do pedido. Sempre positiva.</summary>
public sealed class Quantity : ValueObject
{
    private Quantity(int value) => Value = value;

    public int Value { get; }

    public static Quantity Create(int value)
    {
        if (value <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        return new Quantity(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
