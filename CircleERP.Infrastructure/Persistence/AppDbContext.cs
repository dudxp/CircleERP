using System.Reflection;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace CircleERP.Infrastructure.Persistence;

/// <summary>
/// Implementa <see cref="IUnitOfWork"/>: para as camadas de dentro, este tipo
/// e apenas "algo que confirma alteracoes" -- elas nao conhecem EF Core.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<Currency> Currencies => Set<Currency>();

    internal DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
