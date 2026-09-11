using CircleERP.Application.Abstractions.ZipCodes;
using CircleERP.Domain.Addresses;

namespace CircleERP.Api.IntegrationTests;

/// <summary>
/// Consulta de CEP controlada pelo teste.
/// </summary>
/// <remarks>
/// E por isto que a consulta e uma porta na Application: os testes cobrem os
/// tres desfechos -- achou, nao achou e servico fora -- sem depender da rede
/// nem da disponibilidade do ViaCEP.
/// </remarks>
public sealed class FakeZipCodeLookup : IZipCodeLookup
{
    /// <summary>Devolvido quando o CEP e encontrado. Nulo simula CEP inexistente.</summary>
    public ZipCodeLookupResult? Result { get; set; }

    /// <summary>Quando verdadeiro, simula o servico externo fora do ar.</summary>
    public bool IsUnavailable { get; set; }

    public Task<ZipCodeLookupResult?> FindAsync(
        ZipCode zipCode,
        CancellationToken cancellationToken = default)
    {
        if (IsUnavailable)
            throw new ZipCodeLookupUnavailableException("indisponivel no teste");

        return Task.FromResult(Result);
    }
}
