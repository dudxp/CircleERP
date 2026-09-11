namespace CircleERP.Domain.Addresses;

public interface IAddressRepository
{
    Task<IReadOnlyList<Address>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Address?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Carrega varios enderecos de uma vez. Existe para que a listagem de
    /// clientes resolva os enderecos em uma consulta, e nao uma por cliente.
    /// </summary>
    Task<IReadOnlyList<Address>> GetByIdsAsync(
        IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    void Add(Address address);

    void Remove(Address address);
}
