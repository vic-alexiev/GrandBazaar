using GrandBazaar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public interface IEthereumService
    {
        string GetTransactionUrl(string hash);

        Task<string> PurchaseAsync(
            string keystoreJson,
            string password,
            byte[] itemId,
            long price,
            int quantity);

        Task<string> AddItemAsync(
            string keystoreJson,
            string password,
            byte[] itemId,
            long price,
            int quantity);

        Task<bool> CheckItemExistsAsync(byte[] itemId);

        Task<ItemDetails> GetItemDetailsAsync(byte[] itemId);

        Task<List<byte[]>> GetAllItemsAsync();

        Task<List<byte[]>> GetItemsAsync(string address);

        Task<int> GetItemStockAsync(byte[] itemId);
    }
}
