using FluentResults;
using MediatR;

namespace CircleERP.Application.Abstractions.Messaging;

/// <summary>Caso de uso que apenas le. Nunca altera estado.</summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
