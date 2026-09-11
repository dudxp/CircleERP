using CircleERP.Application.Products;
using CircleERP.Application.Products.ChangeProduct;
using CircleERP.Application.Products.GetProductById;
using CircleERP.Application.Products.GetProducts;
using CircleERP.Application.Products.RegisterProduct;
using CircleERP.Application.Products.SetProductStatus;
using CircleERP.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Controllers;

/// <summary>
/// Borda HTTP do cadastro de produtos.
/// </summary>
/// <remarks>
/// Nao existe DELETE: produto que ja foi vendido nao se apaga, se inativa --
/// as linhas de pedido que o venderam precisam seguir legiveis.
/// </remarks>
[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductsQuery(), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        ProductFields fields,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterProductCommand(fields), cancellationToken);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Altera o produto. Reajustar o preco aqui nao mexe em pedido nenhum: a
    /// linha do pedido guarda o preco praticado na venda.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Change(
        int id,
        ProductFields fields,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeProductCommand(id, fields), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetProductStatusCommand(id, true), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetProductStatusCommand(id, false), cancellationToken);

        return result.ToActionResult(this);
    }
}
