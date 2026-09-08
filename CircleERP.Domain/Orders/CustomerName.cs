using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Orders;

/// <summary>
/// Nome do cliente do pedido.
/// </summary>
/// <remarks>
/// Texto livre, e nao referencia a um cadastro, porque nao existe modulo de
/// clientes ainda. Quando existir, isto vira um <c>CustomerId</c> e o pedido
/// passa a referenciar o agregado Cliente por identidade.
/// </remarks>
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
