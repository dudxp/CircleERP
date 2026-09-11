using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Products;
using CircleERP.Domain.Shared;

namespace CircleERP.Application.Products;

/// <summary>
/// Campos de um produto vindos da borda, compartilhados por cadastro e
/// alteracao.
/// </summary>
public sealed record ProductFields(
    string Sku,
    string Name,
    decimal Price,
    string CurrencyCode,
    string Unit);

internal static class ProductFieldsExtensions
{
    internal static (Sku Sku, ProductName Name, CurrencyCode Currency, UnitOfMeasure Unit)
        ToValueObjects(this ProductFields fields)
    {
        if (!Enum.TryParse<UnitOfMeasure>(fields.Unit, ignoreCase: true, out var unit)
            || !Enum.IsDefined(unit))
        {
            throw new DomainException(
                $"Unidade invalida: {fields.Unit}. Use Unit, Kilogram, Box, Liter ou Meter.");
        }

        return (
            Domain.Products.Sku.Create(fields.Sku),
            ProductName.Create(fields.Name),
            Domain.Currencies.CurrencyCode.Create(fields.CurrencyCode),
            unit
        );
    }

    internal static Money ToPrice(this ProductFields fields, CurrencyCode currency) =>
        Money.Create(fields.Price, currency);
}
