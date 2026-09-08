using CircleERP.Application.Currencies;
using CircleERP.Application.Currencies.ChangeCurrency;
using CircleERP.Application.Currencies.GetCurrencies;
using CircleERP.Application.Currencies.GetCurrencyById;
using CircleERP.Application.Currencies.RegisterCurrency;
using CircleERP.Application.Currencies.RemoveCurrency;
using CircleERP.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Controllers;

/// <summary>
/// Borda HTTP do modulo de moedas. Nao contem regra de negocio: traduz a
/// requisicao para um caso de uso e o resultado para uma resposta.
/// </summary>
[ApiController]
[Route("api/currencies")]
[Produces("application/json")]
public sealed class CurrenciesController(ISender sender) : ControllerBase
{
    /// <summary>Lista todas as moedas cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CurrencyResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCurrenciesQuery(), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Busca uma moeda pelo identificador interno.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CurrencyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCurrencyByIdQuery(id), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Cadastra uma moeda.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        RegisterCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>Altera a descricao e a taxa de cambio de uma moeda.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Change(
        int id,
        ChangeCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeCurrencyCommand(id, request.Description, request.Rate, request.Symbol);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Remove uma moeda.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveCurrencyCommand(id), cancellationToken);

        return result.ToActionResult(this);
    }
}

/// <summary>
/// Corpo do PUT. O id vem da rota, entao nao se repete aqui -- evita a
/// ambiguidade de um id no corpo divergir do id da URL.
/// </summary>
public sealed record ChangeCurrencyRequest(string Description, decimal Rate, string? Symbol);
