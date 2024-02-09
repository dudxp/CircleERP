using Microsoft.EntityFrameworkCore;
using System.Data.Odbc;


namespace ProjectName.ServiceModel.Data
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
            optionsBuilder.Use

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MoedaController>();
        }

        public DbSet<MoedaController> Moedas { get; set; }
    }
}
