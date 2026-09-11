using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Products.GetProducts;

public sealed record GetProductsQuery : IQuery<IReadOnlyList<ProductResponse>>;
