using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DeckHub.Identity.Models;

namespace DeckHub.Identity.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
