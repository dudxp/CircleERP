using CircleERP.Domain.Currencies;
using CircleERP.Domain.Orders;
using CircleERP.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CircleERP.Infrastructure.Persistence.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_item");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(item => item.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(item => item.Description)
            .HasColumnName("description")
            .HasMaxLength(ItemDescription.MaxLength)
            .IsRequired()
            .HasConversion(
                description => description.Value,
                value => ItemDescription.Create(value));

        builder.Property(item => item.Quantity)
            .HasColumnName("quantity")
            .IsRequired()
            .HasConversion(
                quantity => quantity.Value,
                value => Quantity.Create(value));

        // Money e quantia mais moeda. A moeda e sempre a do pedido -- guardar
        // as duas colunas mantem a linha legivel sozinha, sem juntar com o
        // cabecalho para saber em que moeda o preco esta.
        builder.ComplexProperty(item => item.UnitPrice, price =>
        {
            price.Property(money => money.Amount)
                .HasColumnName("unit_price")
                .HasPrecision(18, Money.Scale)
                .IsRequired();

            price.Property(money => money.Currency)
                .HasColumnName("unit_price_currency")
                .HasMaxLength(CurrencyCode.Length)
                .IsRequired()
                .HasConversion(
                    currency => currency.Value,
                    value => CurrencyCode.Create(value));
        });

        builder.Ignore(item => item.LineTotal);
        builder.Ignore(item => item.DomainEvents);
    }
}
