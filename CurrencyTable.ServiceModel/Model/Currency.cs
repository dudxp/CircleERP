using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CurrencyTable.ServiceModel.Model
{
    public class Currency
    {
        [Key]
        [Required]
        public string Code { get; set; }

        public string Description { get; set; }

        public string Rating { get; set; }

        public int Id { get; set; }
        
        //export interface ICurrency
        //{
        //    id: number,
        //    code: string,
        //    description: string,
        //    rating: number
        //}
    }
}
