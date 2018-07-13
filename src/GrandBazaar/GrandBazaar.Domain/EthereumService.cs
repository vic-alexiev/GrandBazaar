using GrandBazaar.Domain.Extensions;
using GrandBazaar.Domain.Models;
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
        private readonly string EtherscanUrl;

        private Web3 _web3;
        private Contract _contract;

        public EthereumService(
            string url,
            string contractAbi,
            string contractAddress,
            string etherscanUrl)
        {
            Url = url;
            ContractAbi = contractAbi;
            ContractAddress = contractAddress;
            EtherscanUrl = etherscanUrl;

            _web3 = new Web3(url);
            _contract = _web3.Eth.GetContract(contractAbi, contractAddress);
        }

        public string GetTransactionUrl(string hash)
        {
            return $"{EtherscanUrl.TrimEnd('/')}/{hash}";
        }

        public async Task<string> PurchaseAsync(
            string keystoreJson,
            string password,
            byte[] itemId,
            long price,
            int quantity)
        {
            Account account = GetAccount(keystoreJson, password);
            Contract contract = GetContract(account);
            Function purchaseFunc = contract.GetFunction("purchase");
            object[] input = new object[] { itemId, quantity };

            long amount = price * quantity;
            string txHash = await purchaseFunc
                .SendTransactionAsync(
                account.Address,
                Gas,
                new HexBigInteger(amount),
                functionInput: input)
                .ConfigureAwait(false);

            return txHash;
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

        public async Task<bool> CheckItemExistsAsync(byte[] itemId)
        {
            ItemDetails itemDetails = await GetItemDetailsAsync(itemId)
                .ConfigureAwait(false);

            return itemDetails.Valid();
        }

        public async Task<ItemDetails> GetItemDetailsAsync(byte[] itemId)
        {
            Function getItemDetailsFunc = _contract.GetFunction("detailsOf");
            object[] input = new object[] { itemId };

            ItemDetails itemDetails = await getItemDetailsFunc
                .CallDeserializingToObjectAsync<ItemDetails>(functionInput: input)
                .ConfigureAwait(false);
            return itemDetails;
        }

        public async Task<List<byte[]>> GetAllItemsAsync()
        {
            Function getItemsFunc = _contract.GetFunction("getAllItems");
            List<byte[]> items = await getItemsFunc
                .CallAsync<List<byte[]>>()
                .ConfigureAwait(false);
            return items;
        }

        public async Task<List<byte[]>> GetItemsAsync(string address)
        {
            Function getItemsFunc = _contract.GetFunction("getItems");
            List<byte[]> items = await getItemsFunc
                .CallAsync<List<byte[]>>(address, Gas, new HexBigInteger(0))
                .ConfigureAwait(false);
            return items;
        }

        public async Task<int> GetItemStockAsync(byte[] itemId)
        {
            Function getItemsFunc = _contract.GetFunction("availability");
            object[] input = new object[] { itemId };

            int stock = await getItemsFunc
                .CallAsync<int>(functionInput: input)
                .ConfigureAwait(false);
            return stock;
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
