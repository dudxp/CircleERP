using CircleERP.Application.Customers;
using CircleERP.Application.Customers.ChangeCustomer;
using CircleERP.Application.Customers.GetCustomerById;
using CircleERP.Application.Customers.GetCustomers;
using CircleERP.Application.Customers.RegisterCustomer;
using CircleERP.Application.Customers.SetCustomerStatus;
using CircleERP.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Controllers;

/// <summary>
/// Borda HTTP do cadastro de clientes.
/// </summary>
/// <remarks>
/// Nao existe DELETE: cliente com historico de pedidos nao se apaga, se
/// inativa. Ativar e inativar sao acoes e por isso aparecem como sub-recursos.
/// </remarks>
[ApiController]
[Route("api/customers")]
[Produces("application/json")]
public sealed class CustomersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CustomerResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomersQuery(), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerByIdQuery(id), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        CustomerFields fields,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterCustomerCommand(fields), cancellationToken);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Change(
        int id,
        CustomerFields fields,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeCustomerCommand(id, fields), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetCustomerStatusCommand(id, true), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetCustomerStatusCommand(id, false), cancellationToken);

        return result.ToActionResult(this);
    }
}
