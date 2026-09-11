using CircleERP.Domain.Currencies;
using CircleERP.Domain.Products;
using CircleERP.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CircleERP.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("product");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(product => product.Sku)
            .HasColumnName("sku")
            .HasMaxLength(Sku.MaxLength)
            .IsRequired()
            .HasConversion(sku => sku.Value, value => Sku.Create(value));

        // Chave natural, como o documento do cliente: a unicidade fica no banco.
        builder.HasIndex(product => product.Sku)
            .IsUnique()
            .HasDatabaseName("IX_product_sku");

        builder.Property(product => product.Name)
            .HasColumnName("name")
            .HasMaxLength(ProductName.MaxLength)
            .IsRequired()
            .HasConversion(name => name.Value, value => ProductName.Create(value));

        // Preco e quantia mais moeda, nas duas colunas: um preco sem moeda nao
        // significa nada.
        builder.ComplexProperty(product => product.Price, price =>
        {
            price.Property(money => money.Amount)
                .HasColumnName("price")
                .HasPrecision(18, Money.Scale)
                .IsRequired();

            price.Property(money => money.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(CurrencyCode.Length)
                .IsRequired()
                .HasConversion(
                    currency => currency.Value,
                    value => CurrencyCode.Create(value));
        });

        builder.Property(product => product.Unit)
            .HasColumnName("unit")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(product => product.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Ignore(product => product.DomainEvents);
    }
}
