using GrandBazaar.Domain.Models;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public interface IIpfsService
    {
        Task<byte[]> AddItemAsync(Item item);
    }
}
