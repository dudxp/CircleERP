using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CircleERP.Model;

[Table("CURRENCY")]
public class Currency
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [Column("CODE")]
    public string Code { get; set; }

    [Column("DESCRIPTION")]
    public string Description { get; set; }

    [Column("RATING")]
    public float Rating { get; set; }
}
