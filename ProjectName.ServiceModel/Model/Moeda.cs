using System.ComponentModel.DataAnnotations;

namespace ProjectName.ServiceModel.Model
{
    public class Moeda
    {
        [Key]
        [Required]
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Taxa_Cambio { get; set; }
    }
}
