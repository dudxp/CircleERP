using CircleERP.Domain.Currencies;

namespace CircleERP.Application.Currencies;

internal static class CurrencyMappings
{
    internal static CurrencyResponse ToResponse(this Currency currency) =>
        new(
            currency.Id,
            currency.Code.Value,
            currency.Description.Value,
            currency.Rate.Value,
            currency.Symbol?.Value);
}
