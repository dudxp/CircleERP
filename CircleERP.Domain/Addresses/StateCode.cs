using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Addresses;

/// <summary>
/// Unidade federativa. Recusa qualquer par de letras que nao seja uma das 27 --
/// "XX" nao e um estado, mesmo tendo o formato certo.
/// </summary>
public sealed class StateCode : ValueObject
{
    public const int Length = 2;

    private static readonly HashSet<string> Valid =
    [
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
        "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
        "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO",
    ];

    private StateCode(string value) => Value = value;

    public string Value { get; }

    /// <summary>As 27 unidades federativas, em ordem alfabetica.</summary>
    public static IReadOnlyCollection<string> All { get; } = [.. Valid.Order()];

    public static StateCode Create(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException("A UF e obrigatoria.");

        if (!Valid.Contains(normalized))
            throw new DomainException($"UF invalida: {normalized}.");

        return new StateCode(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
