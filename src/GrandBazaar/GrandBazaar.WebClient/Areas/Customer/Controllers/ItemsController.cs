using GrandBazaar.Common.Extensions;
using GrandBazaar.Domain;
using GrandBazaar.Domain.Extensions;
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
using System.Threading.Tasks;
using Image = GrandBazaar.WebClient.Areas.Customer.Models.Image;

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
            return await ExecuteInTryCatch(async () =>
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
            }, new List<ItemViewModel>());
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return await ExecuteInTryCatch(async () =>
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception("Item Id must be specified.");
                }

                byte[] itemId = id.HexToByteArray();

                bool itemExists = await EthereumService
                    .CheckItemExistsAsync(itemId)
                    .ConfigureAwait(false);
                if (!itemExists)
                {
                    throw new Exception($"Item with Id {id} does not exist.");
                }

                Item item = await IpfsService.GetItemAsync(itemId).ConfigureAwait(false);
                int quantity = await EthereumService
                    .GetItemAvailabilityAsync(itemId)
                    .ConfigureAwait(false);

                var model = item.ToViewModel(quantity);
                model.Valid = true;
                return View(model);
            }, new ItemViewModel());
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

                byte[] itemId = id.HexToByteArray();

                ItemDetails itemDetails = await EthereumService
                    .GetItemDetailsAsync(itemId)
                    .ConfigureAwait(false);
                if (!itemDetails.Valid())
                {
                    throw new Exception("Invalid item Id.");
                }

                var model = new PurchaseItemViewModel
                {
                    SellerAddress = itemDetails.Seller,
                    Id = id,
                    Price = itemDetails.Price,
                    Valid = true
                };

                int availability = await EthereumService
                    .GetItemAvailabilityAsync(itemId)
                    .ConfigureAwait(false);
                if (availability == 0)
                {
                    ShowError("This item is out of stock.");
                    return View(model);
                }

                model.InStock = true;
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

                    int availability = await EthereumService
                        .GetItemAvailabilityAsync(model.Id.HexToByteArray())
                        .ConfigureAwait(false);
                    if (availability == 0)
                    {
                        throw new Exception("This item is out of stock.");
                    }

                    if (model.Quantity > availability)
                    {
                        throw new Exception($"Not enough stock. Only {availability} piece(s) left.");
                    }

                    string txHash = await EthereumService.PurchaseAsync(
                        keystore.ReadAllText(),
                        model.AccountPassword,
                        model.Id.HexToByteArray(),
                        model.Price,
                        model.Quantity)
                        .ConfigureAwait(false);

                    ShowTransactionInfo(txHash);
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }, model);
        }
    }
}