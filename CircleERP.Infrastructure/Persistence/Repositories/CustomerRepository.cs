using CircleERP.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CircleERP.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await context.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Id)
            .ToListAsync(cancellationToken);

    public async Task<Customer?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await context.Customers
            .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetByIdsAsync(
        IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
            return [];

        return await context.Customers
            .AsNoTracking()
            .Where(customer => ids.Contains(customer.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithDocumentAsync(
        Document document,
        CancellationToken cancellationToken = default) =>
        await context.Customers.AnyAsync(
            customer => customer.Document == document, cancellationToken);

    public async Task<bool> ExistsWithDocumentAsync(
        Document document,
        int exceptId,
        CancellationToken cancellationToken = default) =>
        await context.Customers.AnyAsync(
            customer => customer.Document == document && customer.Id != exceptId,
            cancellationToken);

    public async Task<bool> AnyLinkedToAddressAsync(
        int addressId,
        CancellationToken cancellationToken = default) =>
        await context.Customers.AnyAsync(
            customer => customer.AddressId == addressId, cancellationToken);

    public void Add(Customer customer) => context.Customers.Add(customer);
}
