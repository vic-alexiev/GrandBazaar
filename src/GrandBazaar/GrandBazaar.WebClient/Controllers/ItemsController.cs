using System.Collections.Generic;
using System.Threading.Tasks;
using GrandBazaar.Domain;
using GrandBazaar.Domain.Models;
using GrandBazaar.WebClient.Extensions;
using GrandBazaar.WebClient.Mappers;
using GrandBazaar.WebClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrandBazaar.WebClient.Controllers
{
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
                        .AddItemAsync(keystore.ReadAllText(), model.KeystorePassword, itemId, model.Price, model.Quantity)
                        .ConfigureAwait(false);
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