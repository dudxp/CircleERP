namespace CircleERP.Domain.Currencies;

/// <summary>
/// Acesso ao agregado <see cref="Currency"/>. Declarado no dominio e
/// implementado na infraestrutura, para que a regra de negocio dependa de um
/// contrato seu e nao do EF Core.
/// </summary>
/// <remarks>
/// Os metodos de escrita apenas registram a intencao; quem confirma e o
/// <c>IUnitOfWork</c>, uma unica vez ao final do caso de uso.
/// </remarks>
public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Currency?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Currency?> GetByCodeAsync(CurrencyCode code, CancellationToken cancellationToken = default);

    Task<bool> ExistsWithCodeAsync(CurrencyCode code, CancellationToken cancellationToken = default);

    void Add(Currency currency);

    void Remove(Currency currency);
}
