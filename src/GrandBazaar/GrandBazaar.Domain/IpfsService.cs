using GrandBazaar.Common;
using GrandBazaar.Common.Extensions;
using GrandBazaar.Domain.Models;
using Ipfs;
using Ipfs.Api;
using Nethereum.Hex.HexConvertors.Extensions;
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

        public async Task<Item> GetItemAsync(byte[] itemId)
        {
            MultiHash multiHash = new MultiHash(
                MultiHash.DefaultAlgorithmName,
                itemId);
            string hash = multiHash.ToString();

            Item item = null;
            using (Stream stream = await _ipfsClient
                        .FileSystem
                        .ReadFileAsync(hash)
                        .ConfigureAwait(false))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                byte[] data = memoryStream.ToArray();
                if (!data.IsNullOrEmpty())
                {
                    string content = Encoding.UTF8.GetString(data);
                    item = JsonUtils.Deserialize<Item>(content);
                    item.Id = itemId.ToHex();
                    item.IpfsHash = hash;
                }

                return item;
            }
        }

        public async Task<List<Item>> GetItemsAsync(List<byte[]> itemIds)
        {
            List<Item> items = new List<Item>();

            foreach (byte[] itemId in itemIds)
            {
                Item item = await GetItemAsync(itemId).ConfigureAwait(false);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
