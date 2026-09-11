using CircleERP.Application.Abstractions.Errors;
using FluentResults;

namespace CircleERP.Application.Addresses;

internal static class AddressErrors
{
    internal static Error NotFound(int id) =>
        new NotFoundError($"Endereco {id} nao encontrado.");

    internal static Error LinkedToCustomer(int id) =>
        new ConflictError(
            $"O endereco {id} esta vinculado a um cliente e nao pode ser excluido. " +
            "Desvincule-o do cliente primeiro.");
}
