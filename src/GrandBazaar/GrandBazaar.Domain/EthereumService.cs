using Nethereum.Contracts;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrandBazaar.Domain
{
    public class EthereumService : IEthereumService
    {
        private Contract _contract;

        public EthereumService(string url, string abi, string contractAddress)
        {
            Web3 web3 = new Web3(url);
            _contract = web3.Eth.GetContract(abi, contractAddress);
        }
    }
}
