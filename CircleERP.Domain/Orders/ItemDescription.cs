using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Orders;

/// <summary>
/// O que esta sendo vendido em uma linha do pedido.
/// </summary>
/// <remarks>
/// Texto livre pelo mesmo motivo de <see cref="CustomerName"/>: nao existe
/// catalogo de produtos ainda.
/// </remarks>
public sealed class ItemDescription : ValueObject
{
    public const int MaxLength = 200;

    private ItemDescription(string value) => Value = value;

    public string Value { get; }

    public static ItemDescription Create(string? value)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrEmpty(normalized))
            throw new DomainException("A descricao do item e obrigatoria.");

        if (normalized.Length > MaxLength)
            throw new DomainException($"A descricao do item deve ter no maximo {MaxLength} caracteres.");

        return new ItemDescription(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
