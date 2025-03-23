using Microsoft.EntityFrameworkCore;
using System.Data.Odbc;
using Controllers.Currencys;


namespace CurrencyTable.ServiceModel.Data
{
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
            builder.Entity<CurrencyController>();
        }

        public DbSet<CurrencyController> currencys { get; set; }
    }
}
