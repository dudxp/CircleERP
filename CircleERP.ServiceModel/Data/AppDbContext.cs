using Microsoft.EntityFrameworkCore;
using System.Data.Odbc;
using CircleERP.Model.Controllers.Currencys;
using CircleERP.Model.ServiceModel.Model;

namespace CircleERP.Model.ServiceModel.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "";
        OdbcConnection connection = new OdbcConnection(connectionString);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Currency>();
    }

    public DbSet<Currency> Currencys { get; set; }
}
