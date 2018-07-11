using GrandBazaar.Common;
using GrandBazaar.Common.Extensions;
using GrandBazaar.Domain.Models;
using Ipfs;
using Ipfs.Api;
using System.Collections.Generic;
using System.IO;
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

        public async Task<byte[]> AddItemAsync(Item item)
        {
            string itemSerialized = JsonUtils.Serialize(item, false);
            byte[] itemBytes = Encoding.UTF8.GetBytes(itemSerialized);
            using (var stream = new MemoryStream(itemBytes))
            {
                // Set the position to the beginning of the stream.
                stream.Seek(0, SeekOrigin.Begin);

                IFileSystemNode ipfsNode = await _ipfsClient
                    .FileSystem
                    .AddAsync(stream, item.Name)
                    .ConfigureAwait(false);

                byte[] digest = ipfsNode.Id.Hash.Digest;
                return digest;
            }
        }

        public async Task<List<Item>> GetItemsAsync(List<byte[]> itemDigests)
        {
            List<Item> items = new List<Item>();

            foreach (byte[] digest in itemDigests)
            {
                MultiHash multiHash = new MultiHash(
                    MultiHash.DefaultAlgorithmName, digest);
                string path = multiHash.ToString();

                using (Stream stream = await _ipfsClient
                        .FileSystem
                        .ReadFileAsync(path)
                        .ConfigureAwait(false))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    byte[] data = memoryStream.ToArray();
                    if (!data.IsNullOrEmpty())
                    {
                        string content = Encoding.UTF8.GetString(data);
                        Item item = JsonUtils.Deserialize<Item>(content);
                        item.Id = path;

                        items.Add(item);
                    }
                }
            }

            return items;
        }
    }
}
