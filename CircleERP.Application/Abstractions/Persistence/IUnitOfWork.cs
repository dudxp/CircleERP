namespace CircleERP.Application.Abstractions.Persistence;

/// <summary>
/// Confirma, em uma unica transacao, todas as alteracoes feitas nos agregados
/// durante um caso de uso. Os repositorios apenas registram a intencao; quem
/// grava e o handler, chamando este contrato uma vez ao final.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
