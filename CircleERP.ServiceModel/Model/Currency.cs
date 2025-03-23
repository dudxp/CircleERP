using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CircleERP.Model.ServiceModel.Model;

[Table("CURRENCY")]
public class Currency
{
    [Column("ID")]
    public int Id { get; set; }

    [Key]
    [Required]
    [Column("CODE")]
    public string Code { get; set; }

    [Column("DESCRIPTION")]
    public string Description { get; set; }

    [Column("RATING")]
    public float Rating { get; set; }

    //export interface ICurrency
    //{
    //    id: number,
    //    code: string,
    //    description: string,
    //    rating: number
    //}
}
