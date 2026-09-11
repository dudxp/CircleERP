using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Products;
using FluentResults;

namespace CircleERP.Application.Products.GetProducts;

internal sealed class GetProductsQueryHandler(IProductRepository products)
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductResponse>>
{
    public async Task<Result<IReadOnlyList<ProductResponse>>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await products.GetAllAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<ProductResponse>>(
            [.. result.Select(product => product.ToResponse())]);
    }
}
