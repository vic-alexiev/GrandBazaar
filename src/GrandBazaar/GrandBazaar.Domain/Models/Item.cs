using System.Collections.Generic;

namespace GrandBazaar.Domain.Models
{
    public class Item
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public int Quantity { get; set; }

        public List<Image> Images { get; set; }
    }
}
