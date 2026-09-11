using CircleERP.Domain.Addresses;

namespace CircleERP.Application.Abstractions.ZipCodes;

/// <summary>
/// Consulta de endereco a partir do CEP.
/// </summary>
/// <remarks>
/// Declarado aqui e implementado na infraestrutura: a aplicacao sabe que existe
/// uma forma de consultar CEP, e nao qual servico externo atende. Trocar o
/// provedor, ou usar um falso nos testes, nao toca em nada fora da infra.
/// </remarks>
public interface IZipCodeLookup
{
    /// <summary>
    /// Devolve o endereco do CEP, ou <c>null</c> quando o CEP nao existe.
    /// </summary>
    /// <exception cref="ZipCodeLookupUnavailableException">
    /// O servico nao respondeu. Nao e o mesmo que CEP inexistente: aqui nao se
    /// sabe, e a tela deve oferecer o preenchimento manual.
    /// </exception>
    Task<ZipCodeLookupResult?> FindAsync(
        ZipCode zipCode,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Endereco devolvido pela consulta. Nao inclui numero nem complemento: o CEP
/// nao os conhece.
/// </summary>
/// <remarks>
/// Os campos sao texto cru, e nao value objects: e dado externo, ainda nao
/// validado. Quem decide se vira um endereco valido e o dominio, quando o
/// usuario confirmar o cadastro.
/// </remarks>
public sealed record ZipCodeLookupResult(
    string ZipCode,
    string Street,
    string District,
    string City,
    string State);

public sealed class ZipCodeLookupUnavailableException(string message, Exception? inner = null)
    : Exception(message, inner);
