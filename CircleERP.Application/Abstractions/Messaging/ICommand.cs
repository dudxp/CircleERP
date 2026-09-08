using FluentResults;
using MediatR;

namespace CircleERP.Application.Abstractions.Messaging;

/// <summary>Caso de uso que altera estado.</summary>
public interface ICommand : IRequest<Result>;

/// <summary>Caso de uso que altera estado e devolve um dado (ex.: o id criado).</summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
