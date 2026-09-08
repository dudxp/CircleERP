using CircleERP.Domain.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Middleware;

/// <summary>
/// Uma invariante violada por dado vindo da borda (codigo de moeda com quatro
/// letras, taxa negativa) e erro do cliente, nao falha do servidor: vira 400
/// com ProblemDetails em vez de 500 com stack trace.
/// </summary>
internal sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Requisicao invalida",
                Detail = exception.Message,
            },
        });
    }
}
