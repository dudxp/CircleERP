using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Products;
using FluentResults;

namespace CircleERP.Application.Products.GetProductById;

internal sealed class GetProductByIdQueryHandler(IProductRepository products)
    : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(query.Id, cancellationToken);

        if (product is null)
            return Result.Fail<ProductResponse>(ProductErrors.NotFound(query.Id));

        return Result.Ok(product.ToResponse());
    }
}
