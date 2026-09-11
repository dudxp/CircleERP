namespace CircleERP.Domain.Products;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsWithSkuAsync(Sku sku, CancellationToken cancellationToken = default);

    /// <summary>Usado ao trocar o codigo: ignora o proprio produto.</summary>
    Task<bool> ExistsWithSkuAsync(Sku sku, int exceptId, CancellationToken cancellationToken = default);

    void Add(Product product);
}
