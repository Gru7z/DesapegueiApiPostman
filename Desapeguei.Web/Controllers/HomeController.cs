using Desapeguei.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Desapeguei.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string? categoria)
        {
            ViewData["TituloSite"] = string.IsNullOrWhiteSpace(categoria)
                ? "DESAPEGUEI"
                : categoria.ToUpper();

            ViewData["CategoriaFiltro"] = categoria;

            return View();
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
