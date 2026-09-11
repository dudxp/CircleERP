using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Products;

/// <summary>
/// Codigo do produto. Chave natural do cadastro, como o documento e para o
/// cliente: dois produtos com o mesmo SKU sao o mesmo produto.
/// </summary>
public sealed class Sku : ValueObject
{
    public const int MaxLength = 30;

    private Sku(string value) => Value = value;

    public string Value { get; }

    public static Sku Create(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException("O codigo do produto e obrigatorio.");

        if (normalized.Length > MaxLength)
            throw new DomainException($"O codigo do produto deve ter no maximo {MaxLength} caracteres.");

        // Espaco no meio do codigo e fonte classica de duplicata invisivel.
        if (normalized.Any(char.IsWhiteSpace))
            throw new DomainException("O codigo do produto nao pode conter espacos.");

        return new Sku(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
