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
            _ => (StatusCodes.Status400BadRequest, "Requisicao invalida"),
        };

        return controller.Problem(
            statusCode: statusCode,
            title: title,
            detail: string.Join(" ", errors.Select(error => error.Message)));
    }
}
