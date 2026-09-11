using CircleERP.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CircleERP.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customer");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(customer => customer.Name)
            .HasColumnName("name")
            .HasMaxLength(CustomerName.MaxLength)
            .IsRequired()
            .HasConversion(name => name.Value, value => CustomerName.Create(value));

        // O documento e gravado so com digitos, e a coluna guarda o tipo junto
        // para que a leitura reconstrua o value object sem adivinhar pelo
        // tamanho da string.
        builder.Property(customer => customer.Document)
            .HasColumnName("document")
            .HasMaxLength(14)
            .IsRequired()
            .HasConversion(
                document => document.Value,
                value => Document.Create(value, value.Length == 11
                    ? CustomerType.Individual
                    : CustomerType.Company));

        // Chave natural: dois cadastros com o mesmo documento sao o mesmo
        // cliente. A unicidade fica no banco, e nao so na checagem do handler.
        builder.HasIndex(customer => customer.Document)
            .IsUnique()
            .HasDatabaseName("IX_customer_document");

        // Type e derivado do documento. Fica como coluna somente para consulta
        // e relatorio -- nada o escreve de forma independente.
        builder.Property(customer => customer.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(customer => customer.AddressId)
            .HasColumnName("address_id");

        builder.Property(customer => customer.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Ignore(customer => customer.DomainEvents);
    }
}
