using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;

namespace CircleERP.Domain.Shared;

/// <summary>
/// Valor monetario: quantia e moeda, juntas e inseparaveis.
/// </summary>
/// <remarks>
/// Guardar so o numero e o que permite somar reais com dolares sem ninguem
/// perceber. Aqui a operacao entre moedas diferentes falha, em vez de produzir
/// um resultado errado.
/// </remarks>
public sealed class Money : ValueObject
{
    public const int Scale = 2;

    private Money(decimal amount, CurrencyCode currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public CurrencyCode Currency { get; }

    public static Money Create(decimal amount, CurrencyCode currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        if (amount < 0)
            throw new DomainException("Valor monetario nao pode ser negativo.");

        if (decimal.Round(amount, Scale) != amount)
            throw new DomainException($"Valor monetario deve ter no maximo {Scale} casas decimais.");

        return new Money(amount, currency);
    }

    public static Money Zero(CurrencyCode currency) => Create(0m, currency);

    public Money Add(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (Currency != other.Currency)
            throw new DomainException(
                $"Nao e possivel somar valores em moedas diferentes ({Currency} e {other.Currency}).");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int factor)
    {
        if (factor < 0)
            throw new DomainException("Fator de multiplicacao nao pode ser negativo.");

        return new Money(Amount * factor, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:0.00}";
}
