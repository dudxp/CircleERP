namespace CircleERP.Domain.Orders;

/// <summary>
/// Acesso ao agregado <see cref="Order"/>.
/// </summary>
/// <remarks>
/// Nao existe repositorio de <see cref="OrderItem"/>: o item nao e raiz de
/// agregado, entao e sempre carregado e salvo junto do pedido.
/// </remarks>
public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Carrega o pedido com os itens -- o agregado inteiro ou nada.</summary>
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    void Add(Order order);

    void Remove(Order order);
}
