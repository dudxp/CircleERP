namespace CircleERP.Application.Currencies;

/// <summary>
/// Formato de saida da moeda. Existe para que o agregado nunca seja serializado
/// direto: mudar o dominio nao pode quebrar quem consome a API.
/// </summary>
public sealed record CurrencyResponse(
    int Id,
    string Code,
    string Description,
    decimal Rate,
    string? Symbol);
