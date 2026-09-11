namespace CircleERP.Application.Products;

public sealed record ProductResponse(
    int Id,
    string Sku,
    string Name,
    decimal Price,
    /// <summary>Moeda do preco. O preco nao e um numero solto.</summary>
    string CurrencyCode,
    /// <summary>"Unit", "Kilogram", "Box", "Liter" ou "Meter".</summary>
    string Unit,
    bool IsActive);
