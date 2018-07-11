﻿using GrandBazaar.Common.Extensions;
using GrandBazaar.Domain;
using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Areas.Seller.Models;
using GrandBazaar.WebClient.Extensions;
using GrandBazaar.WebClient.Mappers;
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
    public class ItemsController : Controller
    {
        IIpfsService _ipfsService;
        IEthereumService _ethereumService;

        public ItemsController(IIpfsService ipfsService, IEthereumService ethereumService)
        {
            _ipfsService = ipfsService;
            _ethereumService = ethereumService;
        }

        // GET: Items
        public async Task<ActionResult> Index()
        {
            bool userAccountIsUnlocked =
                HttpContext.Request.Cookies
                    .TryGetValue("web3-account", out string address);
            if (!userAccountIsUnlocked)
            {
                return View();
            }

            List<byte[]> sellerItems = await _ethereumService
                .GetItemsAsync(address)
                .ConfigureAwait(false);
            if (sellerItems.IsNullOrEmpty())
            {
                return View();
            }

            List<Item> items = await _ipfsService
                .GetItemsAsync(sellerItems)
                .ConfigureAwait(false);
            return View(items.ToViewModels());
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(string id)
        {
            byte[] itemId = id.HexToByteArray();
            Item item = await _ipfsService.GetItemAsync(itemId).ConfigureAwait(false);
            int quantity = await _ethereumService
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
                byte[] itemId = await _ipfsService
                    .AddItemAsync(item)
                    .ConfigureAwait(false);
                string txHash = await _ethereumService
                    .AddItemAsync(keystore.ReadAllText(), model.AccountPassword, itemId, model.Price, model.Quantity)
                    .ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Items/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}