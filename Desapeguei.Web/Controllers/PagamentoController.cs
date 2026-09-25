using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Web.Controllers
{
    public class PagamentoController : Controller
    {
        // GET /Pagamento
        public IActionResult Index()
        {
            ViewData["TituloSite"] = "PAGAMENTO";
            return View();
        }

        // GET /Pagamento/CartaoCredito
        public IActionResult CartaoCredito()
        {
            ViewData["TituloSite"] = "PAGAMENTO";
            return View();
        }
    }
}
