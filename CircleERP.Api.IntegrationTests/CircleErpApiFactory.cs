using System.Data.Common;
using CircleERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CircleERP.Api.IntegrationTests;

/// <summary>
/// Sobe a API inteira em memoria e troca o MySQL por um SQLite em memoria.
/// </summary>
/// <remarks>
/// Cada instancia tem seu proprio banco, criado a partir do modelo do EF -- os
/// testes nao compartilham estado nem dependem de um servidor externo. O que
/// eles cobrem e a pilha completa: rota, serializacao, handler, mapeamento e
/// conversores de value object.
///
/// A ressalva: SQLite nao e MySQL. Comportamentos especificos do provider (tipo
/// de coluna, colacao) nao sao verificados aqui.
/// </remarks>
public sealed class CircleErpApiFactory : WebApplicationFactory<Program>
{
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        // O Program.cs exige uma string de conexao para iniciar. O valor nao e
        // usado: o DbContext e substituido logo abaixo.
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:CircleERP"] = "server=localhost;database=test;user=test;password=test",
                ["Database:ServerVersion"] = "5.7.40",
            }));

        builder.ConfigureTestServices(services =>
        {
            RemoveMySqlRegistrations(services);

            // A conexao fica aberta pelo tempo de vida da fabrica: um SQLite
            // ":memory:" e descartado assim que a ultima conexao fecha.
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    /// <summary>Cria o schema a partir do modelo, sem passar por migrations.</summary>
    public void InitializeDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    /// <summary>
    /// Tira o provider MySQL do container. Remover so o
    /// <c>DbContextOptions&lt;AppDbContext&gt;</c> nao basta: o EF registra a
    /// configuracao do provider em descritores proprios, e sobrar qualquer um
    /// deles faz o contexto acusar dois providers registrados.
    /// </summary>
    private static void RemoveMySqlRegistrations(IServiceCollection services)
    {
        var registrations = services
            .Where(descriptor =>
                descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>)
                || descriptor.ServiceType == typeof(DbContextOptions)
                || descriptor.ServiceType == typeof(AppDbContext)
                || (descriptor.ServiceType.FullName?.Contains("DbContextOptionsConfiguration") ?? false))
            .ToList();

        foreach (var registration in registrations)
        {
            services.Remove(registration);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
            _connection = null;
        }
    }
}
