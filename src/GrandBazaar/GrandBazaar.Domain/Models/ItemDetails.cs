using Nethereum.ABI.FunctionEncoding.Attributes;

namespace GrandBazaar.Domain.Models
{
    [FunctionOutput]
    public class ItemDetails
    {
        [Parameter("uint256", "price", 1)]
        public long Price { get; set; }

        [Parameter("address", "seller", 2)]
        public string Seller { get; set; }
    }
}
