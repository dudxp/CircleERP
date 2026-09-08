using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Currencies;

/// <summary>
/// Codigo ISO 4217 de uma moeda ("BRL", "USD"). Normalizado em maiusculas,
/// de modo que a comparacao nunca depende de <c>ToUpper</c> na consulta.
/// </summary>
public sealed class CurrencyCode : ValueObject
{
    public const int Length = 3;

    private CurrencyCode(string value) => Value = value;

    public string Value { get; }

    public static CurrencyCode Create(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException("O codigo da moeda e obrigatorio.");

        if (normalized.Length != Length)
            throw new DomainException($"O codigo da moeda deve ter {Length} letras.");

        if (!normalized.All(char.IsAsciiLetterUpper))
            throw new DomainException("O codigo da moeda deve conter apenas letras.");

        return new CurrencyCode(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
