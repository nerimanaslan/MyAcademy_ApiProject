using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyAcademy_ApiWebUIProject.Models;

namespace MyAcademy_ApiWebUIProject.Controllers
{
    public class HomeController : Controller
    {
        // Site açılışı puan durumuna yönlenir
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Standings");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
