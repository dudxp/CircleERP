namespace CircleERP.Domain.Abstractions;

/// <summary>
/// Objeto sem identidade, definido pelos seus atributos e imutavel: "BRL" e
/// sempre igual a "BRL". E o lugar onde as regras de formato e faixa vivem,
/// garantidas no construtor -- um value object invalido nao chega a existir.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>Atributos que definem a igualdade, na ordem.</summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != GetType()) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj) => obj is ValueObject value && Equals(value);

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Aggregate(new HashCode(), (hash, component) =>
            {
                hash.Add(component);
                return hash;
            })
            .ToHashCode();

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
