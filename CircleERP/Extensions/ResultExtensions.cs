using CircleERP.Application.Abstractions.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Extensions;

/// <summary>
/// Traduz o <see cref="Result"/> dos casos de uso para HTTP em um unico lugar.
/// Sem isso, cada action repete a mesma cadeia de ifs -- e o corpo do erro
/// acaba divergindo entre endpoints.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller) =>
        result.IsSuccess
            ? controller.NoContent()
            : controller.Problem(result.Errors);

    public static IActionResult ToActionResult<TValue>(
        this Result<TValue> result,
        ControllerBase controller) =>
        result.IsSuccess
            ? controller.Ok(result.Value)
            : controller.Problem(result.Errors);

    private static IActionResult Problem(this ControllerBase controller, List<IError> errors)
    {
        var (statusCode, title) = errors switch
        {
            _ when errors.Any(error => error is NotFoundError) =>
                (StatusCodes.Status404NotFound, "Recurso nao encontrado"),
            _ when errors.Any(error => error is ConflictError) =>
                (StatusCodes.Status409Conflict, "Conflito com o estado atual"),
            _ when errors.Any(error => error is UnavailableError) =>
                (StatusCodes.Status503ServiceUnavailable, "Servico indisponivel"),
            _ => (StatusCodes.Status400BadRequest, "Requisicao invalida"),
        };

        var problem = controller.Problem(
            statusCode: statusCode,
            title: title,
            detail: string.Join(" ", errors.Select(error => error.Message)));

        CopyMetadata(errors, problem);

        return problem;
    }

    /// <summary>
    /// Leva os metadados do erro para as extensoes do ProblemDetails.
    /// </summary>
    /// <remarks>
    /// E como o cliente recebe o dado que torna o erro acionavel -- por exemplo,
    /// o id do endereco ja cadastrado, para oferecer o vinculo em vez de apenas
    /// dizer que houve conflito.
    /// </remarks>
    private static void CopyMetadata(List<IError> errors, ObjectResult problem)
    {
        if (problem.Value is not ProblemDetails details)
            return;

        foreach (var (key, value) in errors.SelectMany(error => error.Metadata))
        {
            details.Extensions[key] = value;
        }
    }
}
