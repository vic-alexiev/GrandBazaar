using System;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public interface IIpfsService
    {
        Task<string> AddFileAsync(string imageFilePath);
    }
}
