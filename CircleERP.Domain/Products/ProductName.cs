using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Products;

public sealed class ProductName : ValueObject
{
    public const int MaxLength = 150;

    private ProductName(string value) => Value = value;

    public string Value { get; }

    public static ProductName Create(string? value)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException("O nome do produto e obrigatorio.");

        if (normalized.Length > MaxLength)
            throw new DomainException($"O nome do produto deve ter no maximo {MaxLength} caracteres.");

        return new ProductName(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
