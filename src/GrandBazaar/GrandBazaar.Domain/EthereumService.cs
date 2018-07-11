using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public class EthereumService : IEthereumService
    {
        private static readonly HexBigInteger Gas = new HexBigInteger(4600000);
        private readonly string Url;
        private readonly string ContractAbi;
        private readonly string ContractAddress;

        private Web3 _web3;
        private Contract _contract;

        public EthereumService(string url, string contractAbi, string contractAddress)
        {
            Url = url;
            ContractAbi = contractAbi;
            ContractAddress = contractAddress;

            _web3 = new Web3(url);
            _contract = _web3.Eth.GetContract(contractAbi, contractAddress);
        }

        public async Task<string> AddItemAsync(
            string keystoreJson,
            string password,
            byte[] itemId,
            long price,
            int quantity)
        {
            Account account = GetAccount(keystoreJson, password);
            Contract contract = GetContract(account);
            Function addItemFunc = contract.GetFunction("addItem");
            object[] input = new object[] { itemId, price, quantity };

            string txHash = await addItemFunc
                .SendTransactionAsync(
                account.Address,
                Gas,
                new HexBigInteger(0),
                functionInput: input)
                .ConfigureAwait(false);

            return txHash;
        }

        public async Task<List<byte[]>> GetItemsAsync(string address)
        {
            Function getItemsFunc = _contract.GetFunction("getItems");
            List<byte[]> items = await getItemsFunc
                .CallAsync<List<byte[]>>(address, Gas, new HexBigInteger(0))
                .ConfigureAwait(false);
            return items;
        }

        public async Task<int> GetItemAvailabilityAsync(byte[] itemId)
        {
            Function getItemsFunc = _contract.GetFunction("availability");
            object[] input = new object[] { itemId };

            int availability = await getItemsFunc
                .CallAsync<int>(functionInput: input)
                .ConfigureAwait(false);
            return availability;
        }

        private Account GetAccount(
            string keystoreJson,
            string password)
        {
            Account account = Account.LoadFromKeyStore(keystoreJson, password);
            return account;
        }

        private Contract GetContract(Account account)
        {
            Web3 web3 = new Web3(account, Url);
            Contract contract = web3.Eth.GetContract(ContractAbi, ContractAddress);
            return contract;
        }
    }
}
