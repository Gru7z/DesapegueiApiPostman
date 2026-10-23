
using Desapeguei.Web.Models;
using Desapeguei.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Desapeguei.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProdutosApiClient _produtosApi;
        private readonly IConfiguration _configuration;

        public HomeController(
            ProdutosApiClient produtosApi,
            IConfiguration configuration)
        {
            _produtosApi = produtosApi;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string? categoria)
        {
            ViewData["TituloSite"] = string.IsNullOrWhiteSpace(categoria)
                ? "DESAPEGUEI"
                : categoria.ToUpper();

            ViewData["CategoriaFiltro"] = categoria;
            ViewData["ApiBaseUrl"] = _configuration["ApiBaseUrl"];

            var produtos =
                await _produtosApi.ListarDisponiveisAsync(categoria);

            return View(produtos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
