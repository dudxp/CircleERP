using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Currencies;
using CircleERP.Infrastructure.Persistence;
using CircleERP.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CircleERP.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra o acesso a dados e as implementacoes dos contratos declarados
    /// nas camadas internas. Unico ponto do sistema que conhece EF Core e MySQL.
    /// </summary>
    /// <param name="serverVersion">
    /// Versao do MySQL (ex.: "8.0.36"). Quando omitida, e detectada abrindo uma
    /// conexao durante a inicializacao -- pratico em desenvolvimento, mas
    /// impede a aplicacao de subir se o banco estiver fora.
    /// </param>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string? serverVersion = null)
    {
        var version = string.IsNullOrWhiteSpace(serverVersion)
            ? ServerVersion.AutoDetect(connectionString)
            : ServerVersion.Parse(serverVersion);

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, version));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();

        return services;
    }
}
