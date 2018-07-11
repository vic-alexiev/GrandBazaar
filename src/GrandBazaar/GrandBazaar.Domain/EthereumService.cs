﻿using Nethereum.Contracts;
using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrandBazaar.Domain
{
    public class EthereumService : IEthereumService
    {
        private static readonly HexBigInteger Gas = new HexBigInteger(4600000);
        private readonly string Url;
        private readonly string ContractAbi;
        private readonly string ContractAddress;

        //private Web3 _web3;
        //private Contract _contract;

        public EthereumService(string url, string contractAbi, string contractAddress)
        {
            Url = url;
            ContractAbi = contractAbi;
            ContractAddress = contractAddress;

            //_web3 = new Web3(account, url);
            //_contract = _web3.Eth.GetContract(abi, contractAddress);
        }

        public async Task<string> AddItemAsync(
            string keystoreJson,
            string password,
            byte[] itemId,
            long price,
            int quantity)
        {
            try
            {
                Account account = GetAccount(keystoreJson, password);
                Contract contract = GetContract(account);
                Function addItemFunc = contract.GetFunction("addItem");
                object[] functionInput = new object[] { itemId, price, quantity };

                string txHash = await addItemFunc
                    .SendTransactionAsync(
                    account.Address,
                    Gas,
                    new HexBigInteger(0),
                    functionInput: functionInput)
                    .ConfigureAwait(false);

                return txHash;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
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
