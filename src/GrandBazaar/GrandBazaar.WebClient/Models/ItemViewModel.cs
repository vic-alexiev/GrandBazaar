using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Models
{
    public class ItemViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public int Quantity { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
