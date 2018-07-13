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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Image = GrandBazaar.WebClient.Areas.Seller.Models.Image;


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
            return await ExecuteInTryCatch(async () =>
            {
                string address = GetAccountAddressOrFail();

                List<byte[]> sellerItems = await EthereumService
                    .GetItemsAsync(address)
                    .ConfigureAwait(false);
                if (sellerItems.IsNullOrEmpty())
                {
                    return View(new List<ItemViewModel>());
                }

                List<Item> items = await IpfsService
                    .GetItemsAsync(sellerItems)
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

                    IReadOnlyList<IFormFile> images =
                        HttpContext.Request.Form.Files.GetFiles("images");
                    if (images.All(img => img.Length == 0))
                    {
                        throw new Exception("Your item must have at least one image.");
                    }

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

                    ShowTransactionInfo(txHash);
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }, model);
        }
    }
}