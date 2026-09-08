using Microsoft.Extensions.DependencyInjection;

namespace CircleERP.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra o acesso a dados e as implementacoes dos contratos declarados
    /// nas camadas internas. Unico ponto do sistema que conhece EF Core e MySQL.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Preenchido na Fase 2, junto com a migracao de Currency:
        // DbContext, IEntityTypeConfiguration, repositorios e IUnitOfWork.
        return services;
    }
}
