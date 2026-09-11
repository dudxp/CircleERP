using CircleERP.Domain.Addresses;

namespace CircleERP.Application.Addresses;

/// <summary>
/// Campos de um endereco vindos da borda.
/// </summary>
/// <remarks>
/// Compartilhado por cadastro e alteracao: os dois recebem exatamente o mesmo
/// conjunto, e a conversao para value objects acontece uma vez so.
/// </remarks>
public sealed record AddressFields(
    string ZipCode,
    string Street,
    string Number,
    string? Complement,
    string District,
    string City,
    string State);

internal static class AddressFieldsExtensions
{
    internal static (ZipCode ZipCode, AddressText Street, AddressText Number,
        AddressText? Complement, AddressText District, AddressText City, StateCode State)
        ToValueObjects(this AddressFields fields) =>
        (
            Domain.Addresses.ZipCode.Create(fields.ZipCode),
            AddressText.Create(fields.Street, "Logradouro", Address.StreetMaxLength),
            AddressText.Create(fields.Number, "Numero", Address.NumberMaxLength),
            AddressText.CreateOrNull(fields.Complement, "Complemento", Address.ComplementMaxLength),
            AddressText.Create(fields.District, "Bairro", Address.DistrictMaxLength),
            AddressText.Create(fields.City, "Cidade", Address.CityMaxLength),
            StateCode.Create(fields.State)
        );
}
