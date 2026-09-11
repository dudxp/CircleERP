using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Addresses;

/// <summary>
/// Trecho textual do endereco (logradouro, bairro, cidade, numero,
/// complemento).
/// </summary>
/// <remarks>
/// Um unico tipo para todos eles, parametrizado pelo rotulo e pelo tamanho, em
/// vez de cinco value objects quase identicos. O que muda entre eles e so a
/// mensagem de erro e o limite -- copiar a mesma classe cinco vezes seria
/// duplicacao sem ganho.
/// </remarks>
public sealed class AddressText : ValueObject
{
    private AddressText(string value) => Value = value;

    public string Value { get; }

    public static AddressText Create(string? value, string label, int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException($"{label} e obrigatorio.");

        if (normalized.Length > maxLength)
            throw new DomainException($"{label} deve ter no maximo {maxLength} caracteres.");

        return new AddressText(normalized);
    }

    /// <summary>Versao opcional: ausencia e um estado valido.</summary>
    public static AddressText? CreateOrNull(string? value, string label, int maxLength)
    {
        var normalized = value?.Trim();

        return string.IsNullOrEmpty(normalized)
            ? null
            : Create(normalized, label, maxLength);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
