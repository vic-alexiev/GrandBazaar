﻿using GrandBazaar.Common;
using GrandBazaar.Domain.Mappers;
using GrandBazaar.Domain.Models;
using Ipfs;
using Ipfs.Api;
using Ipfs.CoreApi;
using System;
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

        public async Task<string> AddItemAsync(Item item)
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

                string hash = ipfsNode.Id.Hash.ToString();
                return hash;
            }
        }
    }
}