using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public interface IEthereumService
    {
        Task<string> AddItemAsync(
            string keystoreJson,
            string keystorePassword,
            byte[] itemId,
            long price,
            int quantity);

        Task<List<byte[]>> GetItemsAsync(string address);

        Task<int> GetItemAvailabilityAsync(byte[] itemId);
    }
}
