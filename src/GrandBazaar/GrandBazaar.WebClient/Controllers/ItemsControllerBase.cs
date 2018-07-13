using GrandBazaar.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandBazaar.WebClient.Controllers
{
    public abstract class ItemsControllerBase : Controller
    {
        protected IIpfsService IpfsService;
        protected IEthereumService EthereumService;

        private string _accountAddress;

        protected ItemsControllerBase(
            IIpfsService ipfsService,
            IEthereumService ethereumService)
        {
            IpfsService = ipfsService;
            EthereumService = ethereumService;
        }

        public void SetAccountAddress(string address)
        {
            _accountAddress = address;
        }

        public string GetAccountAddressOrFail()
        {
            if (string.IsNullOrWhiteSpace(_accountAddress))
            {
                throw new Exception("Account address unavailable. Please, log in MetaMask.");
            }
            return _accountAddress;
        }

        public async Task<ActionResult> ExecuteInTryCatch(
            Func<Task<ActionResult>> block,
            object model)
        {
            try
            {
                return await block();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }
    }
}
