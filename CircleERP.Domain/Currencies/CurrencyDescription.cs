using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Currencies;

/// <summary>Nome da moeda por extenso ("Real brasileiro").</summary>
public sealed class CurrencyDescription : ValueObject
{
    public const int MaxLength = 100;

    private CurrencyDescription(string value) => Value = value;

    public string Value { get; }

    public static CurrencyDescription Create(string? value)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException("A descricao da moeda e obrigatoria.");

        if (normalized.Length > MaxLength)
            throw new DomainException($"A descricao da moeda deve ter no maximo {MaxLength} caracteres.");

        return new CurrencyDescription(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
