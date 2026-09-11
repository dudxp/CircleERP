namespace CircleERP.Domain.Customers;

/// <summary>
/// Natureza do cliente. Define qual documento ele precisa ter: pessoa fisica
/// usa CPF, pessoa juridica usa CNPJ.
/// </summary>
public enum CustomerType
{
    Individual = 1,
    Company = 2,
}
