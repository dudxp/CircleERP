using CircleERP.Domain.Addresses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CircleERP.Infrastructure.Persistence.Configurations;

internal sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("address");

        builder.HasKey(address => address.Id);

        builder.Property(address => address.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(address => address.ZipCode)
            .HasColumnName("zip_code")
            .HasMaxLength(ZipCode.Length)
            .IsRequired()
            .HasConversion(zipCode => zipCode.Value, value => ZipCode.Create(value));

        MapText(builder, address => address.Street, "street", Address.StreetMaxLength, "Logradouro");
        MapText(builder, address => address.Number, "number", Address.NumberMaxLength, "Numero");
        MapText(builder, address => address.District, "district", Address.DistrictMaxLength, "Bairro");
        MapText(builder, address => address.City, "city", Address.CityMaxLength, "Cidade");

        // Opcional: ausencia de complemento e um estado valido.
        builder.Property(address => address.Complement)
            .HasColumnName("complement")
            .HasMaxLength(Address.ComplementMaxLength)
            .IsRequired(false)
            .HasConversion(
                text => text!.Value,
                value => AddressText.CreateOrNull(value, "Complemento", Address.ComplementMaxLength));

        builder.Property(address => address.State)
            .HasColumnName("state")
            .HasMaxLength(StateCode.Length)
            .IsRequired()
            .HasConversion(state => state.Value, value => StateCode.Create(value));

        builder.Ignore(address => address.DomainEvents);
    }

    private static void MapText(
        EntityTypeBuilder<Address> builder,
        System.Linq.Expressions.Expression<Func<Address, AddressText>> property,
        string columnName,
        int maxLength,
        string label)
    {
        builder.Property(property)
            .HasColumnName(columnName)
            .HasMaxLength(maxLength)
            .IsRequired()
            .HasConversion(
                text => text.Value,
                value => AddressText.Create(value, label, maxLength));
    }
}
