using CircleERP.Application.Abstractions.Errors;
using FluentResults;

namespace CircleERP.Application.Addresses;

internal static class AddressErrors
{
    internal static Error NotFound(int id) =>
        new NotFoundError($"Endereco {id} nao encontrado.");

    /// <summary>
    /// Endereco identico ja cadastrado. Carrega o id do existente nos metadados
    /// para que a tela possa oferecer o vinculo em vez de so recusar.
    /// </summary>
    internal static Error AlreadyExists(int existingId, string singleLine) =>
        new ConflictError($"Este endereco ja esta cadastrado: {singleLine}.")
            .WithMetadata("existingAddressId", existingId);

    internal static Error ZipCodeNotFound(string formattedZipCode) =>
        new NotFoundError($"CEP {formattedZipCode} nao encontrado.");

    internal static Error LookupUnavailable(string reason) =>
        new UnavailableError(
            $"Nao foi possivel consultar o CEP ({reason}). Preencha o endereco manualmente.");

    internal static Error LinkedToCustomer(int id) =>
        new ConflictError(
            $"O endereco {id} esta vinculado a um cliente e nao pode ser excluido. " +
            "Desvincule-o do cliente primeiro.");
}
