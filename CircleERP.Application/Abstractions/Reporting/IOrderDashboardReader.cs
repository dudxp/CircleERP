namespace CircleERP.Application.Abstractions.Reporting;

/// <summary>
/// Leitura agregada dos pedidos, para o painel.
/// </summary>
/// <remarks>
/// Nao e um repositorio: repositorio carrega e salva agregados inteiros, e aqui
/// nada e carregado nem alterado -- sao somas e contagens feitas no banco. Misturar
/// as duas coisas acabaria enchendo o `IOrderRepository` de metodos de relatorio
/// que nunca devolvem um `Order`.
///
/// Por isso tambem os numeros vem do banco, e nao de somar em memoria a lista
/// inteira de pedidos: a agregacao cresce com o cadastro, o trafego nao.
/// </remarks>
public interface IOrderDashboardReader
{
    Task<OrderDashboard> ReadAsync(CancellationToken cancellationToken = default);
}

/// <summary>Numeros do painel, ja agregados.</summary>
public sealed record OrderDashboard(
    int TotalOrders,
    IReadOnlyList<OrderCountByStatus> ByStatus,
    IReadOnlyList<OrderCountByCustomer> ByCustomer,
    IReadOnlyList<OrderCountByDate> ByDate,
    IReadOnlyList<OrderTotalByCurrency> TotalsByCurrency);

public sealed record OrderCountByStatus(string Status, int OrderCount);

public sealed record OrderCountByCustomer(int CustomerId, string Customer, int OrderCount);

/// <summary>Pedidos abertos em cada dia.</summary>
public sealed record OrderCountByDate(DateOnly Date, int OrderCount);

/// <summary>
/// Valor total por moeda.
/// </summary>
/// <remarks>
/// Separado por moeda de proposito, e nunca somado num numero unico: um total
/// que junta reais com dolares nao significa nada. E a mesma regra que o
/// <c>Money</c> impoe no dominio.
/// </remarks>
public sealed record OrderTotalByCurrency(string Currency, decimal Total, int OrderCount);
