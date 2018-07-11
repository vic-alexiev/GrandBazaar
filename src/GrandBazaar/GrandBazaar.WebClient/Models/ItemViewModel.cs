using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Models
{
    public class ItemViewModel
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
        public string KeystorePassword { get; set; }
    }
}
