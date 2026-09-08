using CircleERP.Application.Abstractions.Errors;
using FluentResults;

namespace CircleERP.Application.Currencies;

/// <summary>
/// Erros de aplicacao do modulo de moedas, num lugar so -- mensagem duplicada
/// em varios handlers e como mensagem divergente esperando para acontecer.
/// </summary>
internal static class CurrencyErrors
{
    internal static Error NotFound(int id) =>
        new NotFoundError($"Moeda {id} nao encontrada.");

    internal static Error CodeAlreadyRegistered(string code) =>
        new ConflictError($"Ja existe uma moeda cadastrada com o codigo {code}.");
}
