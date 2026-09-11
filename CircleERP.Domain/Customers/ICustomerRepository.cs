namespace CircleERP.Domain.Customers;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Carrega varios clientes de uma vez. Existe para que a listagem de
    /// pedidos resolva os nomes em uma consulta, e nao uma por pedido.
    /// </summary>
    Task<IReadOnlyList<Customer>> GetByIdsAsync(
        IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsWithDocumentAsync(
        Document document,
        CancellationToken cancellationToken = default);

    /// <summary>Usado ao trocar o documento: ignora o proprio cliente.</summary>
    Task<bool> ExistsWithDocumentAsync(
        Document document,
        int exceptId,
        CancellationToken cancellationToken = default);

    /// <summary>Impede excluir um endereco que algum cliente ainda usa.</summary>
    Task<bool> AnyLinkedToAddressAsync(
        int addressId,
        CancellationToken cancellationToken = default);

    void Add(Customer customer);
}
