using FluentResults;

namespace CircleERP.Application.Abstractions.Errors;

/// <summary>Recurso pedido nao existe. A borda traduz para 404.</summary>
public sealed class NotFoundError(string message) : Error(message);
