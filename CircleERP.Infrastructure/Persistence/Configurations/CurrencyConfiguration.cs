using CircleERP.Domain.Currencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CircleERP.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento de <see cref="Currency"/>. Fica aqui, e nao em atributos na
/// entidade, para que o dominio nao carregue anotacoes de persistencia.
/// </summary>
internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("CURRENCY");

        builder.HasKey(currency => currency.Id);

        builder.Property(currency => currency.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(currency => currency.Code)
            .HasColumnName("CODE")
            .HasMaxLength(CurrencyCode.Length)
            .IsRequired()
            .HasConversion(
                code => code.Value,
                value => CurrencyCode.Create(value));

        // O codigo e a chave natural: a unicidade e garantida pelo banco, e nao
        // apenas pela verificacao previa do handler (que sofre corrida).
        builder.HasIndex(currency => currency.Code)
            .IsUnique()
            .HasDatabaseName("IX_CURRENCY_CODE");

        builder.Property(currency => currency.Description)
            .HasColumnName("DESCRIPTION")
            .HasMaxLength(CurrencyDescription.MaxLength)
            .IsRequired()
            .HasConversion(
                description => description.Value,
                value => CurrencyDescription.Create(value));

        builder.Property(currency => currency.Rate)
            .HasColumnName("RATING")
            .HasPrecision(18, ExchangeRate.Scale)
            .IsRequired()
            .HasConversion(
                rate => rate.Value,
                value => ExchangeRate.Create(value));

        builder.Ignore(currency => currency.DomainEvents);
    }
}
