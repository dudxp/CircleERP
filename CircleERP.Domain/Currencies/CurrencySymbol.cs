using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Currencies;

/// <summary>
/// Simbolo de exibicao da moeda ("R$", "US$", "EUR"). Diferente do
/// <see cref="CurrencyCode"/>: o codigo identifica a moeda em integracoes e e
/// padronizado pela ISO 4217; o simbolo existe para ser mostrado na tela.
/// </summary>
/// <remarks>
/// Opcional: nem toda moeda cadastrada precisa de um simbolo conhecido. Quando
/// ausente, cabe a interface exibir o codigo.
/// </remarks>
public sealed class CurrencySymbol : ValueObject
{
    public const int MaxLength = 5;

    private CurrencySymbol(string value) => Value = value;

    public string Value { get; }

    /// <summary>
    /// Cria o simbolo, ou devolve <c>null</c> quando nao ha simbolo informado --
    /// ausencia e um estado valido, nao um erro.
    /// </summary>
    public static CurrencySymbol? CreateOrNull(string? value)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrEmpty(normalized))
            return null;

        if (normalized.Length > MaxLength)
            throw new DomainException($"O simbolo da moeda deve ter no maximo {MaxLength} caracteres.");

        return new CurrencySymbol(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
