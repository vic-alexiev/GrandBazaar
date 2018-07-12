using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Areas.Customer.Models
{
    public class PurchaseItemViewModel
    {
        public string Id { get; set; }

        [Required, Range(1, long.MaxValue)]
        public long Price { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }
    }
}
