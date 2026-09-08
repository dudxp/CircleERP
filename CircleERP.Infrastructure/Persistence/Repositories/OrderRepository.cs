using CircleERP.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace CircleERP.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<IReadOnlyList<Order>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await context.Orders
            .AsNoTracking()
            // Include mesmo na listagem: o total do resumo e calculado a partir
            // das linhas, entao um pedido sem os itens carregados apareceria
            // valendo zero.
            .Include(order => order.Items)
            .OrderByDescending(order => order.Id)
            .ToListAsync(cancellationToken);

    /// <summary>Carrega o agregado inteiro -- pedido e itens, nunca so o cabecalho.</summary>
    public async Task<Order?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await context.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public void Add(Order order) => context.Orders.Add(order);

    public void Remove(Order order) => context.Orders.Remove(order);
}
