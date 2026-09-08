using FluentResults;

namespace CircleERP.Application.Abstractions.Errors;

/// <summary>
/// A operacao conflita com o estado atual (ex.: codigo ja cadastrado).
/// A borda traduz para 409.
/// </summary>
public sealed class ConflictError(string message) : Error(message);
