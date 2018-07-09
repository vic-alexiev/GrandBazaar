using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GrandBazaar.Domain;
using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Mappers;
using GrandBazaar.WebClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrandBazaar.WebClient.Controllers
{
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
        public ActionResult Index()
        {
            return View();
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
        public async Task<ActionResult> Create(ItemViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                try
                {
                    IFormFileCollection images = Request.Form.Files;
                    Item item = model.ToDomainModel(images);
                    string itemHash = await _ipfsService.AddItemAsync(item).ConfigureAwait(false);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(model);
                }
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