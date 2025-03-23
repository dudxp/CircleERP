using System.ComponentModel.DataAnnotations;

namespace CircleERP.Model.ServiceModel.Data.Dto.Currency;

public class ReadCurrencyDto
{
    [Key]
    [Required]
    public string Code { get; set; }
    public int Id { get; set; }
    public string Description { get; set; }
    public string Rating { get; set; }
}
