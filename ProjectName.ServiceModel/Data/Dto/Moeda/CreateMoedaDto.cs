using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectName.ServiceModel.Data.Dto.Moeda
{
    public class CreateMoedaDto
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Taxa_Cambio { get; set; }
    }
}
