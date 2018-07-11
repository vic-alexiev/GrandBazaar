using GrandBazaar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public interface IIpfsService
    {
        Task<byte[]> AddItemAsync(Item item);

        Task<Item> GetItemAsync(byte[] itemId);

        Task<List<Item>> GetItemsAsync(List<byte[]> itemDigests);
    }
}
