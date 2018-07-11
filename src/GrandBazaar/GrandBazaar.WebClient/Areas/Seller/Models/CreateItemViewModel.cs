using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Areas.Seller.Models
{
    public class CreateItemViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required, Range(1, long.MaxValue)]
        public long Price { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }
    }
}
