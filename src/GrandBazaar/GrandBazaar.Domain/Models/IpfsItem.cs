namespace GrandBazaar.Domain.Models
{
    public class IpfsItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Price { get; set; }

        public int Quantity { get; set; }

        public string[] Images { get; set; }
    }
}
