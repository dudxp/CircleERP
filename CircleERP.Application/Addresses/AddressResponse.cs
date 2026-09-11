namespace CircleERP.Application.Addresses;

public sealed record AddressResponse(
    int Id,
    string ZipCode,
    string FormattedZipCode,
    string Street,
    string Number,
    string? Complement,
    string District,
    string City,
    string State,
    /// <summary>Endereco montado em uma linha, para listagem e seletor.</summary>
    string SingleLine);
