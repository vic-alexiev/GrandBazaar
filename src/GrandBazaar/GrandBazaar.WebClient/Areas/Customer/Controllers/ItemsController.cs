using GrandBazaar.Common.Extensions;
using GrandBazaar.Domain;
using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Areas.Customer.Mappers;
using GrandBazaar.WebClient.Areas.Customer.Models;
using GrandBazaar.WebClient.Controllers;
using GrandBazaar.WebClient.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace GrandBazaar.WebClient.Areas.Customer.Controllers
{
    [Area("Customer")]
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
            List<byte[]> allItems = await EthereumService
                .GetAllItemsAsync()
                .ConfigureAwait(false);
            if (allItems.IsNullOrEmpty())
            {
                return View(new List<ItemViewModel>());
            }

            List<Item> items = await IpfsService
                .GetItemsAsync(allItems)
                .ConfigureAwait(false);
            return View(items.ToViewModels());
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                byte[] itemId = id.HexToByteArray();
                Item item = await IpfsService.GetItemAsync(itemId).ConfigureAwait(false);
                int quantity = await EthereumService
                    .GetItemAvailabilityAsync(itemId)
                    .ConfigureAwait(false);

                return View(item.ToViewModel(quantity));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new ItemViewModel());
            }
        }

        // GET: Items/Purchase/5
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Purchase(string id)
        {
            return await ExecuteInTryCatch(async () =>
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception("Item Id must be specified.");
                }

                ItemDetails itemDetails = await EthereumService
                    .GetItemDetailsAsync(id.HexToByteArray())
                    .ConfigureAwait(false);
                if (itemDetails.Seller.HexToBigInteger(true) == BigInteger.Zero)
                {
                    throw new Exception("Invalid item Id.");
                }

                var model = new PurchaseItemViewModel
                {
                    SellerAddress = itemDetails.Seller,
                    Id = id,
                    Price = itemDetails.Price
                };
                return View(model);
            }, new PurchaseItemViewModel());
        }

        // POST: Items/Purchase/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Purchase(PurchaseItemViewModel model, string returnUrl = null)
        {
            return await ExecuteInTryCatch(async () =>
            {
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    IFormFile keystore =
                        HttpContext.Request.Form.Files.GetFile("keystore");
                    if (keystore.Length == 0)
                    {
                        throw new Exception("Keystore file must be specified.");
                    }

                    string txHash = await EthereumService.PurchaseAsync(
                        keystore.ReadAllText(),
                        model.AccountPassword,
                        model.Id.HexToByteArray(),
                        model.Price,
                        model.Quantity)
                        .ConfigureAwait(false);
                    string txUrl = EthereumService.GetTransactionUrl(txHash);

                    TempData["TxInfo"] = txUrl;
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }, model);
        }
    }
}