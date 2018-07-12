using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public interface IEthereumService
    {
        string GetTxUrl(string txHash);

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

        Task<List<byte[]>> GetAllItemsAsync();

        Task<List<byte[]>> GetItemsAsync(string address);

        Task<int> GetItemAvailabilityAsync(byte[] itemId);
    }
}
