using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Addresses;

/// <summary>
/// CEP. Guarda apenas digitos, para que "01310-100" e "01310100" sejam o mesmo
/// CEP.
/// </summary>
public sealed class ZipCode : ValueObject
{
    public const int Length = 8;

    private ZipCode(string value) => Value = value;

    /// <summary>Somente digitos.</summary>
    public string Value { get; }

    public static ZipCode Create(string? value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

        if (digits.Length == 0)
            throw new DomainException("O CEP e obrigatorio.");

        if (digits.Length != Length)
            throw new DomainException($"O CEP deve ter {Length} digitos.");

        return new ZipCode(digits);
    }

    public string ToFormattedString() => $"{Value[..5]}-{Value[5..]}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
