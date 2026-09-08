using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies.Events;

namespace CircleERP.Domain.Currencies;

/// <summary>
/// Moeda aceita pelo sistema. Raiz de agregado: um pedido referencia uma moeda
/// pelo <see cref="CurrencyCode"/>, nunca por uma instancia compartilhada.
/// </summary>
public sealed class Currency : Entity<int>, IAggregateRoot
{
    /// <summary>Exigido pelo EF Core.</summary>
    private Currency()
    {
        Code = null!;
        Description = null!;
        Rate = null!;
    }

    private Currency(
        CurrencyCode code,
        CurrencyDescription description,
        ExchangeRate rate,
        CurrencySymbol? symbol)
    {
        Code = code;
        Description = description;
        Rate = rate;
        Symbol = symbol;
    }

    public CurrencyCode Code { get; private set; }

    public CurrencyDescription Description { get; private set; }

    public ExchangeRate Rate { get; private set; }

    /// <summary>Simbolo de exibicao. Ausente quando nao se conhece um.</summary>
    public CurrencySymbol? Symbol { get; private set; }

    /// <summary>
    /// Unica forma de criar uma moeda. Como os value objects ja validaram a si
    /// mesmos, nao existe instancia de <see cref="Currency"/> em estado invalido.
    /// </summary>
    public static Currency Register(
        CurrencyCode code,
        CurrencyDescription description,
        ExchangeRate rate,
        CurrencySymbol? symbol = null)
    {
        var currency = new Currency(code, description, rate, symbol);

        currency.Raise(new CurrencyRegistered(code.Value, DateTime.UtcNow));

        return currency;
    }

    public void ChangeDescription(CurrencyDescription description) => Description = description;

    public void ChangeSymbol(CurrencySymbol? symbol) => Symbol = symbol;

    public void ChangeRate(ExchangeRate rate)
    {
        if (Rate == rate)
            return;

        var previous = Rate;
        Rate = rate;

        Raise(new CurrencyRateChanged(Id, previous.Value, rate.Value, DateTime.UtcNow));
    }
}
