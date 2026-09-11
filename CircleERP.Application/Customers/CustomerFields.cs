using CircleERP.Domain.Customers;

namespace CircleERP.Application.Customers;

/// <summary>
/// Campos de um cliente vindos da borda, compartilhados por cadastro e
/// alteracao.
/// </summary>
/// <remarks>
/// O tipo entra aqui porque a borda precisa dizer se o documento deve ser lido
/// como CPF ou CNPJ. No agregado ele deixa de ser um campo proprio: passa a ser
/// derivado do documento ja validado.
/// </remarks>
public sealed record CustomerFields(
    string Name,
    string Type,
    string Document,
    int? AddressId);

internal static class CustomerFieldsExtensions
{
    internal static (CustomerName Name, Document Document) ToValueObjects(this CustomerFields fields)
    {
        if (!Enum.TryParse<CustomerType>(fields.Type, ignoreCase: true, out var type)
            || !Enum.IsDefined(type))
        {
            throw new Domain.Abstractions.DomainException(
                $"Tipo de cliente invalido: '{fields.Type}'. Use 'Individual' ou 'Company'.");
        }

        return (CustomerName.Create(fields.Name), Document.Create(fields.Document, type));
    }
}
