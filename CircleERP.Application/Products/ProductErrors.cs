using CircleERP.Application.Abstractions.Errors;
using FluentResults;

namespace CircleERP.Application.Products;

internal static class ProductErrors
{
    internal static Error NotFound(int id) =>
        new NotFoundError($"Produto {id} nao encontrado.");

    internal static Error SkuAlreadyRegistered(string sku) =>
        new ConflictError($"Ja existe um produto cadastrado com o codigo {sku}.");

    internal static Error Inactive(string name) =>
        new ConflictError($"O produto {name} esta inativo e nao pode ser vendido.");
}
