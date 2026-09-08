using CircleERP.Application.Orders;
using CircleERP.Application.Orders.AddOrderItem;
using CircleERP.Application.Orders.CancelOrder;
using CircleERP.Application.Orders.ChangeOrderItemQuantity;
using CircleERP.Application.Orders.GetOrderById;
using CircleERP.Application.Orders.GetOrders;
using CircleERP.Application.Orders.OpenOrder;
using CircleERP.Application.Orders.PlaceOrder;
using CircleERP.Application.Orders.RemoveOrderItem;
using CircleERP.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Controllers;

/// <summary>
/// Borda HTTP do modulo de pedidos.
/// </summary>
/// <remarks>
/// O pedido nasce em rascunho, recebe itens um a um e so entao e confirmado.
/// Confirmar e cancelar sao acoes, nao alteracoes de campo, e por isso aparecem
/// como sub-recursos e nao como um PATCH em `status` -- assim nao existe
/// requisicao capaz de pular uma etapa do ciclo.
/// </remarks>
[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    /// <summary>Lista os pedidos, do mais recente para o mais antigo.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrderSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrdersQuery(), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Busca um pedido com seus itens.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Abre um pedido em rascunho, sem itens.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Open(
        OpenOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>Adiciona uma linha ao pedido. So enquanto ele estiver em rascunho.</summary>
    [HttpPost("{id:int}/items")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(
        int id,
        AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddOrderItemCommand(
            id,
            request.Description,
            request.Quantity,
            request.UnitPrice);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return CreatedAtAction(nameof(GetById), new { id }, result.Value);
    }

    /// <summary>Altera a quantidade de uma linha.</summary>
    [HttpPut("{id:int}/items/{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeItemQuantity(
        int id,
        int itemId,
        ChangeOrderItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeOrderItemQuantityCommand(id, itemId, request.Quantity);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Remove uma linha do pedido.</summary>
    [HttpDelete("{id:int}/items/{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(
        int id,
        int itemId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveOrderItemCommand(id, itemId), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Confirma o pedido. Recusado se ele nao tiver itens.</summary>
    [HttpPost("{id:int}/place")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Place(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new PlaceOrderCommand(id), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Cancela o pedido.</summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CancelOrderCommand(id), cancellationToken);

        return result.ToActionResult(this);
    }
}

/// <summary>Corpo do POST de item. O id do pedido vem da rota.</summary>
public sealed record AddOrderItemRequest(string Description, int Quantity, decimal UnitPrice);

/// <summary>Corpo do PUT de item.</summary>
public sealed record ChangeOrderItemQuantityRequest(int Quantity);
