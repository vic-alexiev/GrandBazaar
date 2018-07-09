using GrandBazaar.Domain.Models;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public interface IIpfsService
    {
        Task<string> AddItemAsync(Item item);
    }
}
