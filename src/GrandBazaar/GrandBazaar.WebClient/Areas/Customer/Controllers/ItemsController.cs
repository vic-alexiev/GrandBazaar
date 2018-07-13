﻿using GrandBazaar.Common.Extensions;
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
                return View();
            }

            List<Item> items = await IpfsService
                .GetItemsAsync(allItems)
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

        // GET: Items/Purchase?id=xxx&price=100000
        [Authorize(Roles = "Customer")]
        public ActionResult Purchase(string id, long price)
        {
            // TODO: fix
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception("Item Id must be specified.");
            }
            if (price <= 0)
            {
                throw new Exception("Item price must be greater than 0.");
            }

            var model = new PurchaseItemViewModel
            {
                Id = id,
                Price = price
            };
            return View(model);
        }

        // POST: Items/Purchase/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Purchase(PurchaseItemViewModel model, string returnUrl = null)
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
        }
    }
}