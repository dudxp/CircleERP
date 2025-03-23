using Microsoft.AspNetCore.Mvc;
using CircleERP.Model.ServiceModel.Model;
using System.Collections.Generic;
using CircleERP.Model.Model.Services;

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
        // Your implementation to retrieve currencies from the database or other source
        //List<CurrencyController> currencies = new List<CurrencyController>();

        var currencys = _service.GetAllCurrency();

        return Ok(currencys);
    } 
}
