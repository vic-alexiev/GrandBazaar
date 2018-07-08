using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GrandBazaar.WebClient.Models;
using GrandBazaar.Domain;

namespace GrandBazaar.WebClient.Controllers
{
    public class HomeController : Controller
    {
        IIpfsService _ipfsService;

        public HomeController(IIpfsService ipfsService)
        {
            _ipfsService = ipfsService;
        }

        public async Task<IActionResult> Index()
        {
            string hash = await _ipfsService.AddFileAsync(@"C:\temp\album\butterfly.png").ConfigureAwait(false);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
