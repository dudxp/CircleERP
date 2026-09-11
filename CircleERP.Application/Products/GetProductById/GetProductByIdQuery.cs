using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Products.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IQuery<ProductResponse>;
