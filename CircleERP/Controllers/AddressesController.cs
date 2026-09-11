using CircleERP.Application.Addresses;
using CircleERP.Application.Addresses.ChangeAddress;
using CircleERP.Application.Addresses.GetAddressById;
using CircleERP.Application.Addresses.GetAddresses;
using CircleERP.Application.Addresses.RegisterAddress;
using CircleERP.Application.Addresses.RemoveAddress;
using CircleERP.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Controllers;

/// <summary>
/// Borda HTTP do cadastro de enderecos.
/// </summary>
/// <remarks>
/// Endereco tem cadastro proprio, e nao vive dentro do cliente: o mesmo
/// endereco pode ser cadastrado antes de existir qualquer cliente, e a tela de
/// cliente apenas vincula um que ja exista.
/// </remarks>
[ApiController]
[Route("api/addresses")]
[Produces("application/json")]
public sealed class AddressesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AddressResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAddressesQuery(), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<AddressResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAddressByIdQuery(id), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        AddressFields fields,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterAddressCommand(fields), cancellationToken);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Change(
        int id,
        AddressFields fields,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeAddressCommand(id, fields), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>Remove um endereco. Recusado se algum cliente o estiver usando.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remove(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveAddressCommand(id), cancellationToken);

        return result.ToActionResult(this);
    }
}
