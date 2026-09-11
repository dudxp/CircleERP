using CircleERP.Domain.Products;

namespace CircleERP.Application.Products;

internal static class ProductMappings
{
    internal static ProductResponse ToResponse(this Product product) =>
        new(
            product.Id,
            product.Sku.Value,
            product.Name.Value,
            product.Price.Amount,
            product.Price.Currency.Value,
            product.Unit.ToString(),
            product.IsActive);
}
