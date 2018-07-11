using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandBazaar.Common.Extensions;
using GrandBazaar.Domain;
using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Areas.Customer.Mappers;
using GrandBazaar.WebClient.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrandBazaar.WebClient.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
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

            List<byte[]> allItems = await EthereumService
                .GetAllItemsAsync(address)
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
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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