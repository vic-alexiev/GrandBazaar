using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Areas.Customer.Models
{
    public class PurchaseItemViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required, Range(1, long.MaxValue)]
        public long Price { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }

        public string SellerAddress { get; set; }

        public bool InStock { get; set; }

        public bool Valid { get; set; }
    }
}
