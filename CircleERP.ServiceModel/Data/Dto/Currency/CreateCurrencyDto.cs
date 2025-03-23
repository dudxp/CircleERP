using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleERP.Model.ServiceModel.Data.Dto.Currency;

public class CreateCurrencyDto
{
    public string Code { get; set; }
    public string Description { get; set; }
    public string Rating { get; set; }
}
