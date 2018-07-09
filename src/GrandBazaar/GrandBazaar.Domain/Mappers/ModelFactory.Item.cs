using GrandBazaar.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrandBazaar.Domain.Mappers
{
    public static class ModelFactory
    {
        public static IpfsItem ToIpfsItem(this Item item, List<string> imageHashes)
        {
            IpfsItem ipfsItem = new IpfsItem
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price.ToString(),
                Quantity = item.Quantity,
                Images = imageHashes.ToArray()
            };
            return ipfsItem;
        }
    }
}
