using CircleERP.Application.Addresses;

namespace CircleERP.Application.Customers;

public sealed record CustomerResponse(
    int Id,
    string Name,
    /// <summary>Somente digitos.</summary>
    string Document,
    /// <summary>Com pontuacao, para exibicao.</summary>
    string FormattedDocument,
    /// <summary>"Individual" ou "Company", derivado do documento.</summary>
    string Type,
    bool IsActive,
    int? AddressId,
    /// <summary>Endereco resolvido, quando ha um vinculado.</summary>
    AddressResponse? Address);
