using GrandBazaar.Common.Extensions;
using GrandBazaar.Domain;
using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Areas.Seller.Mappers;
using GrandBazaar.WebClient.Areas.Seller.Models;
using GrandBazaar.WebClient.Controllers;
using GrandBazaar.WebClient.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrandBazaar.WebClient.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class ItemsController : ItemsControllerBase
    {
        public ItemsController(
            IIpfsService ipfsService,
            IEthereumService ethereumService)
            : base(ipfsService, ethereumService)
        {
        }

        // GET: Items
        public async Task<ActionResult> Index()
        {
            string address = GetAccountAddressOrFail();

            List<byte[]> sellerItems = await EthereumService
                .GetItemsAsync(address)
                .ConfigureAwait(false);
            if (sellerItems.IsNullOrEmpty())
            {
                return View();
            }

            List<Item> items = await IpfsService
                .GetItemsAsync(sellerItems)
                .ConfigureAwait(false);
            return View(items.ToViewModels());
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(string id)
        {
            byte[] itemId = id.HexToByteArray();
            Item item = await IpfsService.GetItemAsync(itemId).ConfigureAwait(false);
            int quantity = await EthereumService
                .GetItemAvailabilityAsync(itemId)
                .ConfigureAwait(false);

            return View(item.ToViewModel(quantity));
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateItemViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                IFormFile keystore =
                    HttpContext.Request.Form.Files.GetFile("keystore");
                if (keystore.Length == 0)
                {
                    return View(model);
                }

                IReadOnlyList<IFormFile> images =
                    HttpContext.Request.Form.Files.GetFiles("images");
                Item item = model.ToDomainModel(images);
                byte[] itemId = await IpfsService
                    .AddItemAsync(item)
                    .ConfigureAwait(false);

                string txHash = await EthereumService.AddItemAsync(
                    keystore.ReadAllText(),
                    model.AccountPassword,
                    itemId,
                    model.Price,
                    model.Quantity)
                    .ConfigureAwait(false);
                string txUrl = EthereumService.GetTransactionUrl(txHash);

                TempData["TxInfo"] = txUrl;
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}