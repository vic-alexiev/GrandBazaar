using System.Collections.Generic;

namespace GrandBazaar.WebClient.Areas.Customer.Models
{
    public class ItemViewModel
    {
        public string Id { get; set; }

        public string IpfsHash { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public int Quantity { get; set; }

        public List<Image> Images { get; set; }

        public bool Valid { get; set; }
    }
}
