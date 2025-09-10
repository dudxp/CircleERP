using Microsoft.AspNetCore.Mvc;
using CircleERP.Model;
using System.Collections.Generic;
using CircleERP.Model.Services;
using CircleERP.Model.Data.Dto.Currency;
using FluentResults;

namespace CircleERP.Model.Controllers.Currencys;

[ApiController]
[Route("api/currency")]
public class CurrencyController : ControllerBase
{
    private readonly CurrencyService _service;
    public CurrencyController(CurrencyService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get a list of currencies.
    /// </summary>
    /// <returns>A list of currencies.</returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Currency>))]
    public IActionResult Get()
    {
        var currencys = _service.GetAllCurrencys();
        return Ok(currencys);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateCurrencyDto currency)
    {
        Result<Currency> result = _service.PostCurrency(currency);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value.Id);
    }

    [HttpPut("update-by-id/{id}")]
    public IActionResult Put(int id, [FromBody] UpdateCurrencyDto currency)
    {
        Result result = _service.UpdateCurrency(id, currency);
        if (result.IsFailed) 
            return BadRequest(result.Errors);
        
        return Ok("Moeda atualizada com sucesso");
    }

    [HttpPut("update-by-code/{code}")]
    public IActionResult Put(string code, [FromBody] UpdateCurrencyDto currency)
    {
        Result result = _service.UpdateCurrency(code, currency);
        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok("Moeda atualizada com sucesso");
    }

    [HttpDelete("delete-by-id/{id}")]
    public IActionResult DeleteId(int id)
    {
        Result result = _service.DeleteCurrency(id);
        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok("Moeda deletada com sucesso");
    }

    [HttpDelete("delete-by-code/{code}")]
    public IActionResult DeleteCode(string code)
    {
        Result result = _service.DeleteCurrency(code);
        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok("Moeda deletada com sucesso");
    }
}
