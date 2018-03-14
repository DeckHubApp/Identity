using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Slidable.Identity.Models;

namespace Slidable.Identity.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return LocalRedirect("/");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
