using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTable.ServiceModel.Data.Dto.Currency
{
    public class ReadMoedaDto
    {
        [Key]
        [Required]
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Taxa_Cambio { get; set; }
    }
}
