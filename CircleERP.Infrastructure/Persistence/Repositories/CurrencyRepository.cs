using CircleERP.Domain.Currencies;
using Microsoft.EntityFrameworkCore;

namespace CircleERP.Infrastructure.Persistence.Repositories;

internal sealed class CurrencyRepository(AppDbContext context) : ICurrencyRepository
{
    public async Task<IReadOnlyList<Currency>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await context.Currencies
            .AsNoTracking()
            .OrderBy(currency => currency.Id)
            .ToListAsync(cancellationToken);

    public async Task<Currency?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await context.Currencies
            .FirstOrDefaultAsync(currency => currency.Id == id, cancellationToken);

    public async Task<Currency?> GetByCodeAsync(
        CurrencyCode code,
        CancellationToken cancellationToken = default) =>
        await context.Currencies
            .FirstOrDefaultAsync(currency => currency.Code == code, cancellationToken);

    public async Task<bool> ExistsWithCodeAsync(
        CurrencyCode code,
        CancellationToken cancellationToken = default) =>
        await context.Currencies
            .AnyAsync(currency => currency.Code == code, cancellationToken);

    public void Add(Currency currency) => context.Currencies.Add(currency);

    public void Remove(Currency currency) => context.Currencies.Remove(currency);
}
