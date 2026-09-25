using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Web.Controllers
{
    [Authorize] // só usuário logado pode anunciar um produto
    public class VenderController : Controller
    {
        public IActionResult Index()
        {
            ViewData["TituloSite"] = "DESAPEGUEI";
            return View();
        }
    }
}
