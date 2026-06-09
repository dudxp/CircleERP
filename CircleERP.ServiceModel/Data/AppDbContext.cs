using Microsoft.EntityFrameworkCore;

namespace CircleERP.Model.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt){}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Currency>();
    }

    public DbSet<Currency> Currencys { get; set; }
}
