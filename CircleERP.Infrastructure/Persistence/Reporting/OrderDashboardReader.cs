using CircleERP.Application.Abstractions.Reporting;
using CircleERP.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace CircleERP.Infrastructure.Persistence.Reporting;

/// <summary>
/// Monta os numeros do painel com agregacao no banco.
/// </summary>
/// <remarks>
/// Le direto do <see cref="AppDbContext"/>, sem passar por repositorio: o que
/// sai daqui sao contagens e somas, nunca agregados. Materializar todos os
/// pedidos para soma-los em memoria funcionaria hoje, com poucas linhas, e
/// pararia de funcionar exatamente quando o painel ficasse util.
/// </remarks>
internal sealed class OrderDashboardReader(AppDbContext context) : IOrderDashboardReader
{
    public async Task<OrderDashboard> ReadAsync(CancellationToken cancellationToken = default)
    {
        var orders = context.Orders.AsNoTracking();

        var totalOrders = await orders.CountAsync(cancellationToken);

        var byStatus = await orders
            .GroupBy(order => order.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        var byCustomer = await orders
            .GroupBy(order => order.CustomerId)
            .Select(group => new { CustomerId = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        var byDate = await orders
            .GroupBy(order => order.CreatedOnUtc.Date)
            .Select(group => new { Date = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        // O total de um pedido e a soma das linhas, e nao uma coluna, entao a
        // soma por moeda passa pelos itens.
        var totalsByCurrency = await context.Orders
            .AsNoTracking()
            .Select(order => new
            {
                Currency = order.Currency,
                Total = order.Items.Sum(item => item.UnitPrice.Amount * item.Quantity.Value),
            })
            .GroupBy(row => row.Currency)
            .Select(group => new
            {
                Currency = group.Key,
                Total = group.Sum(row => row.Total),
                Count = group.Count(),
            })
            .ToListAsync(cancellationToken);

        var customerNames = await ReadCustomerNamesAsync(
            [.. byCustomer.Select(row => row.CustomerId)],
            cancellationToken);

        return new OrderDashboard(
            totalOrders,
            [.. byStatus
                .OrderBy(row => row.Status)
                .Select(row => new OrderCountByStatus(row.Status.ToString(), row.Count))],
            [.. byCustomer
                .OrderByDescending(row => row.Count)
                .Select(row => new OrderCountByCustomer(
                    row.CustomerId,
                    customerNames.GetValueOrDefault(row.CustomerId, $"(cliente {row.CustomerId})"),
                    row.Count))],
            [.. byDate
                .OrderBy(row => row.Date)
                .Select(row => new OrderCountByDate(DateOnly.FromDateTime(row.Date), row.Count))],
            [.. totalsByCurrency
                .OrderBy(row => row.Currency.Value)
                .Select(row => new OrderTotalByCurrency(row.Currency.Value, row.Total, row.Count))]);
    }

    /// <remarks>
    /// Uma consulta para todos os nomes, e nao uma por cliente do agrupamento.
    /// </remarks>
    private async Task<Dictionary<int, string>> ReadCustomerNamesAsync(
        IReadOnlyCollection<int> customerIds,
        CancellationToken cancellationToken)
    {
        if (customerIds.Count == 0)
            return [];

        var rows = await context.Customers
            .AsNoTracking()
            .Where(customer => customerIds.Contains(customer.Id))
            .Select(customer => new { customer.Id, customer.Name })
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(row => row.Id, row => row.Name.Value);
    }
}
