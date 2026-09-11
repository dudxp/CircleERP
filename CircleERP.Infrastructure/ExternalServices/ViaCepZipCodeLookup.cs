using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CircleERP.Application.Abstractions.ZipCodes;
using CircleERP.Domain.Addresses;
using Microsoft.Extensions.Logging;

namespace CircleERP.Infrastructure.ExternalServices;

/// <summary>
/// Consulta de CEP no ViaCEP.
/// </summary>
/// <remarks>
/// Servico publico e gratuito, sem contrato de disponibilidade. Por isso toda
/// falha vira <see cref="ZipCodeLookupUnavailableException"/> em vez de subir
/// como erro do servidor: a consulta e uma conveniencia, e o cadastro continua
/// possivel na mao.
/// </remarks>
internal sealed class ViaCepZipCodeLookup(
    HttpClient httpClient,
    ILogger<ViaCepZipCodeLookup> logger) : IZipCodeLookup
{
    public async Task<ZipCodeLookupResult?> FindAsync(
        ZipCode zipCode,
        CancellationToken cancellationToken = default)
    {
        ViaCepResponse? response;

        try
        {
            response = await httpClient.GetFromJsonAsync<ViaCepResponse>(
                $"ws/{zipCode.Value}/json/",
                cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(exception, "Consulta do CEP {ZipCode} falhou.", zipCode.Value);

            throw new ZipCodeLookupUnavailableException("servico de CEP indisponivel", exception);
        }

        // CEP inexistente devolve 200 com { "erro": true }, e nao 404.
        if (response is null || response.IsError)
            return null;

        return new ZipCodeLookupResult(
            zipCode.Value,
            response.Street ?? string.Empty,
            response.District ?? string.Empty,
            response.City ?? string.Empty,
            response.State ?? string.Empty);
    }

    /// <remarks>
    /// O campo <c>erro</c> as vezes vem como booleano e as vezes como a string
    /// "true", dependendo da rota. Ler como <see cref="object"/> aceita os dois.
    /// </remarks>
    private sealed record ViaCepResponse
    {
        [JsonPropertyName("logradouro")]
        public string? Street { get; init; }

        [JsonPropertyName("bairro")]
        public string? District { get; init; }

        [JsonPropertyName("localidade")]
        public string? City { get; init; }

        [JsonPropertyName("uf")]
        public string? State { get; init; }

        [JsonPropertyName("erro")]
        public object? Error { get; init; }

        public bool IsError =>
            Error is not null
            && !string.Equals(Error.ToString(), "false", StringComparison.OrdinalIgnoreCase);
    }
}
