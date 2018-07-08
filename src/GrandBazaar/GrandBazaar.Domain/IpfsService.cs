using Ipfs;
using Ipfs.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public class IpfsService : IIpfsService
    {
        private IpfsClient _ipfsClient;

        public IpfsService()
        {
            _ipfsClient = new IpfsClient();
        }

        public async Task<string> AddFileAsync(string imageFilePath)
        {
            IFileSystemNode ipfsNode = 
                await _ipfsClient.FileSystem.AddFileAsync(imageFilePath).ConfigureAwait(false);
            string hash = ipfsNode.Id.Hash.ToString();
            return hash;
        }
    }
}
