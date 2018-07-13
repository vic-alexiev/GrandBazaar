using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Areas.Customer.Models
{
    public class PurchaseItemViewModel
    {
        [Required, MinLength(64), MaxLength(64)]
        public string Id { get; set; }

        [Required, Range(1, long.MaxValue)]
        public long Price { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, MaxLength(255)]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }

        [Required, MinLength(40), MaxLength(42)]
        public string SellerAddress { get; set; }

        public bool InStock { get; set; }

        public bool Valid { get; set; }
    }
}
