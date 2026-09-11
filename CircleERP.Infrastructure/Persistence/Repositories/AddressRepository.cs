using CircleERP.Domain.Addresses;
using Microsoft.EntityFrameworkCore;

namespace CircleERP.Infrastructure.Persistence.Repositories;

internal sealed class AddressRepository(AppDbContext context) : IAddressRepository
{
    public async Task<IReadOnlyList<Address>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await context.Addresses
            .AsNoTracking()
            .OrderBy(address => address.Id)
            .ToListAsync(cancellationToken);

    public async Task<Address?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await context.Addresses
            .FirstOrDefaultAsync(address => address.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Address>> GetByIdsAsync(
        IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
            return [];

        return await context.Addresses
            .AsNoTracking()
            .Where(address => ids.Contains(address.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Addresses.AnyAsync(address => address.Id == id, cancellationToken);

    public async Task<Address?> FindDuplicateAsync(
        ZipCode zipCode,
        AddressText number,
        AddressText? complement,
        int? exceptId = null,
        CancellationToken cancellationToken = default) =>
        await context.Addresses
            .AsNoTracking()
            .Where(address => exceptId == null || address.Id != exceptId)
            .Where(address => address.ZipCode == zipCode && address.Number == number)
            // Complemento ausente e gravado como NULL; compara-lo direto com um
            // value object nulo nao traduz para SQL, entao os dois casos sao
            // separados.
            .Where(address => complement == null
                ? address.Complement == null
                : address.Complement == complement)
            .FirstOrDefaultAsync(cancellationToken);

    public void Add(Address address) => context.Addresses.Add(address);

    public void Remove(Address address) => context.Addresses.Remove(address);
}
