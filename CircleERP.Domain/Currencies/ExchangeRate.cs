using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Currencies;

/// <summary>
/// Taxa de conversao da moeda. Usa <see cref="decimal"/>, e nao ponto
/// flutuante binario: em float, 0.1 nao e exatamente 0.1, e o erro se acumula
/// a cada conversao de valor monetario.
/// </summary>
public sealed class ExchangeRate : ValueObject
{
    public const int Scale = 6;

    private ExchangeRate(decimal value) => Value = value;

    public decimal Value { get; }

    public static ExchangeRate Create(decimal value)
    {
        if (value <= 0)
            throw new DomainException("A taxa de cambio deve ser maior que zero.");

        if (decimal.Round(value, Scale) != value)
            throw new DomainException($"A taxa de cambio deve ter no maximo {Scale} casas decimais.");

        return new ExchangeRate(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
