using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Customers;

/// <summary>Nome ou razao social do cliente.</summary>
public sealed class CustomerName : ValueObject
{
    public const int MaxLength = 120;

    private CustomerName(string value) => Value = value;

    public string Value { get; }

    public static CustomerName Create(string? value)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException("O nome do cliente e obrigatorio.");

        if (normalized.Length > MaxLength)
            throw new DomainException($"O nome do cliente deve ter no maximo {MaxLength} caracteres.");

        return new CustomerName(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
