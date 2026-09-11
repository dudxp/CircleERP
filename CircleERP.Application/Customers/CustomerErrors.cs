using CircleERP.Application.Abstractions.Errors;
using FluentResults;

namespace CircleERP.Application.Customers;

internal static class CustomerErrors
{
    internal static Error NotFound(int id) =>
        new NotFoundError($"Cliente {id} nao encontrado.");

    internal static Error DocumentAlreadyRegistered(string formattedDocument) =>
        new ConflictError($"Ja existe um cliente cadastrado com o documento {formattedDocument}.");

    internal static Error Inactive(string name) =>
        new ConflictError($"O cliente {name} esta inativo e nao pode receber pedidos.");
}
