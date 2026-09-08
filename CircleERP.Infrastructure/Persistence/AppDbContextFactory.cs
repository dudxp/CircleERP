using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CircleERP.Infrastructure.Persistence;

/// <summary>
/// Usada apenas pelas ferramentas de linha de comando (`dotnet ef`). Fixa a
/// versao do servidor em vez de detecta-la, para que gerar uma migration nao
/// dependa de um MySQL no ar.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string FallbackConnectionString =
        "server=localhost;port=3306;database=circleerp;user=root;password=";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
                               ?? FallbackConnectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(5, 7, 40)))
            .Options;

        return new AppDbContext(options);
    }
}
