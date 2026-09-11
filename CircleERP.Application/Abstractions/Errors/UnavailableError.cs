using FluentResults;

namespace CircleERP.Application.Abstractions.Errors;

/// <summary>
/// Um servico do qual dependemos nao respondeu. A borda traduz para 503.
/// </summary>
/// <remarks>
/// Diferente de 400 e 404: nao ha nada errado com o pedido do cliente, e tentar
/// de novo mais tarde pode funcionar.
/// </remarks>
public sealed class UnavailableError(string message) : Error(message);
