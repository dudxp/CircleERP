using CircleERP.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace CircleERP.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await context.Products
            .AsNoTracking()
            .OrderBy(product => product.Sku)
            .ToListAsync(cancellationToken);

    public async Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await context.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

    public async Task<bool> ExistsWithSkuAsync(
        Sku sku,
        CancellationToken cancellationToken = default) =>
        await context.Products.AnyAsync(product => product.Sku == sku, cancellationToken);

    public async Task<bool> ExistsWithSkuAsync(
        Sku sku,
        int exceptId,
        CancellationToken cancellationToken = default) =>
        await context.Products.AnyAsync(
            product => product.Sku == sku && product.Id != exceptId,
            cancellationToken);

    public void Add(Product product) => context.Products.Add(product);
}
