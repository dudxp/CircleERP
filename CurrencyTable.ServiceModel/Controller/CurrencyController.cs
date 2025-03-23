using Microsoft.AspNetCore.Mvc;
using CurrencyTable.ServiceModel.Model;
using System.Collections.Generic;

namespace Controllers.Currencys;

[ApiController]
[Route("api/currency")]
public class CurrencyController : ControllerBase
{
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

        var currencys = new StaticCurrencys();

        return Ok(currencys.GetCurrencys());
    } 
}

public class StaticCurrencys
{
    public List<Currency> GetCurrencys()
    {
        return new List<Currency>
        {
            new Currency
            {
                Code = "R$",
                Description = "Real",
                Rating = "5.00",
                Id = 1
            },
            new Currency
            {
                Code = "US$",
                Description = "Dollar",
                Rating = "1.00",
                Id = 2
            },
            new Currency
            {
                Code = "€",
                Description = "Euro",
                Rating = "1.10",
                Id = 3
            }
        };
    }
}
