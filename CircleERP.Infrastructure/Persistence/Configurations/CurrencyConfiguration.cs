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
        // Nomes em minusculas, como no banco. Nesta maquina
        // lower_case_table_names=1 torna a comparacao insensivel, mas em Linux
        // o padrao e 0 e "CURRENCY" deixaria de encontrar a tabela.
        builder.ToTable("currency");

        builder.HasKey(currency => currency.Id);

        builder.Property(currency => currency.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(currency => currency.Code)
            .HasColumnName("code")
            .HasMaxLength(CurrencyCode.Length)
            .IsRequired()
            .HasConversion(
                code => code.Value,
                value => CurrencyCode.Create(value));

        // O codigo e a chave natural: a unicidade e garantida pelo banco, e nao
        // apenas pela verificacao previa do handler (que sofre corrida).
        builder.HasIndex(currency => currency.Code)
            .IsUnique()
            .HasDatabaseName("IX_currency_code");

        builder.Property(currency => currency.Description)
            .HasColumnName("description")
            .HasMaxLength(CurrencyDescription.MaxLength)
            .IsRequired()
            .HasConversion(
                description => description.Value,
                value => CurrencyDescription.Create(value));

        builder.Property(currency => currency.Rate)
            .HasColumnName("rating")
            .HasPrecision(18, ExchangeRate.Scale)
            .IsRequired()
            .HasConversion(
                rate => rate.Value,
                value => ExchangeRate.Create(value));

        // Opcional: ausencia de simbolo e um estado valido, entao a coluna aceita
        // nulo e o conversor so roda quando ha valor.
        builder.Property(currency => currency.Symbol)
            .HasColumnName("symbol")
            .HasMaxLength(CurrencySymbol.MaxLength)
            .IsRequired(false)
            .HasConversion(
                symbol => symbol!.Value,
                value => CurrencySymbol.CreateOrNull(value));

        builder.Ignore(currency => currency.DomainEvents);
    }
}
