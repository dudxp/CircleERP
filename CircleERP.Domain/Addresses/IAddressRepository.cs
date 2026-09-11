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

    /// <summary>
    /// Procura um endereco igual a este -- mesmo CEP, numero e complemento.
    /// </summary>
    /// <param name="exceptId">
    /// Ignora este id. Usado na alteracao: um endereco nao e duplicata de si
    /// mesmo.
    /// </param>
    Task<Address?> FindDuplicateAsync(
        ZipCode zipCode,
        AddressText number,
        AddressText? complement,
        int? exceptId = null,
        CancellationToken cancellationToken = default);

    void Add(Address address);

    void Remove(Address address);
}
