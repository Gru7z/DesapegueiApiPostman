using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Web.Controllers
{
    public class CarrinhoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}