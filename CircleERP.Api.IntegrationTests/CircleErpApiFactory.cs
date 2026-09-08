using System.Data.Common;
using CircleERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
    /// <summary>
    /// Nunca e usado para conectar: existe so para o Program.cs conseguir
    /// iniciar antes de o DbContext ser trocado por SQLite.
    /// </summary>
    private const string PlaceholderConnectionString =
        "server=localhost;database=circleerp_test;user=test;password=test";

    private DbConnection? _connection;

    /// <summary>
    /// Fornece, por variavel de ambiente, a configuracao que o Program.cs le
    /// antes de o host existir.
    /// </summary>
    /// <remarks>
    /// Com hosting minimo, o que a fabrica registra em
    /// <c>ConfigureAppConfiguration</c> so e aplicado em <c>Build()</c> -- ou
    /// seja, depois de os top-level statements do Program.cs ja terem lido
    /// <c>builder.Configuration</c> e lancado por falta de string de conexao.
    /// Variaveis de ambiente, ao contrario, entram nas fontes padrao de
    /// configuracao e chegam a tempo.
    ///
    /// `Database__ServerVersion` importa tanto quanto a string de conexao: sem
    /// ele, <c>AddInfrastructure</c> cai no <c>ServerVersion.AutoDetect</c>, que
    /// abre conexao com o MySQL durante a inicializacao.
    /// </remarks>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("MYSQL_CONNECTION_STRING", PlaceholderConnectionString);
        Environment.SetEnvironmentVariable("Database__ServerVersion", "5.7.40");

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

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
