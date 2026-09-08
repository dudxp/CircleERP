using CircleERP.Domain.Currencies;
using CircleERP.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CircleERP.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("order_header");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(order => order.Customer)
            .HasColumnName("customer")
            .HasMaxLength(CustomerName.MaxLength)
            .IsRequired()
            .HasConversion(
                customer => customer.Value,
                value => CustomerName.Create(value));

        // Guarda apenas o codigo da moeda, sem chave estrangeira para currency:
        // um pedido e outro agregado, e referencia por identidade. Amarrar por
        // FK impediria excluir uma moeda que qualquer pedido antigo ja usou.
        builder.Property(order => order.Currency)
            .HasColumnName("currency_code")
            .HasMaxLength(CurrencyCode.Length)
            .IsRequired()
            .HasConversion(
                currency => currency.Value,
                value => CurrencyCode.Create(value));

        builder.Property(order => order.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(order => order.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.Property(order => order.PlacedOnUtc)
            .HasColumnName("placed_on_utc");

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            // Item nao existe sem pedido: apagar o pedido apaga as linhas.
            .OnDelete(DeleteBehavior.Cascade);

        // A colecao e exposta como IReadOnlyList sobre o campo _items; o EF
        // precisa escrever no campo, e nao na propriedade.
        builder.Metadata
            .FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Total e calculado a partir das linhas. Total gravado e total que
        // pode divergir dos itens.
        builder.Ignore(order => order.Total);
        builder.Ignore(order => order.DomainEvents);
    }
}
