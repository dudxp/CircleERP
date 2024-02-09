using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using System.Collections.Generic;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
public class MoedaController : ControllerBase
{
    /// <summary>
    /// Get a list of currencies.
    /// </summary>
    /// <returns>A list of currencies.</returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<MoedaController>))]
    public IActionResult GetCurrencies()
    {
        // Your implementation to retrieve currencies from the database or other source
        List<MoedaController> currencies = new List<MoedaController>();

        return Ok(currencies);
    }
}

public class Moeda
{
    /// <summary>
    /// Currency code.
    /// </summary>
    public string Sigla_Moeda { get; set; }

    /// <summary>
    /// Currency description.
    /// </summary>
    public string Descricao { get; set; }

    /// <summary>
    /// Exchange rate for the currency.
    /// </summary>
    public decimal Taxa_Cambio { get; set; }
}
