
using Desapeguei.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Web.Controllers
{
    [Authorize]
    public class VenderController : Controller
    {
        private readonly ProdutosApiClient _produtosApi;

        public VenderController(ProdutosApiClient produtosApi)
        {
            _produtosApi = produtosApi;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["TituloSite"] = "DESAPEGUEI";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(110_000_000)]
        public async Task<IActionResult> Publicar(
            [FromForm] string Titulo,
            [FromForm] string Descricao,
            [FromForm] string Categoria,
            [FromForm] string Preco,
            [FromForm] IFormFile? Imagem,
            [FromForm] IFormFile? Video)
        {
            if (string.IsNullOrWhiteSpace(Titulo) ||
                string.IsNullOrWhiteSpace(Descricao) ||
                string.IsNullOrWhiteSpace(Categoria))
            {
                return BadRequest(new
                {
                    mensagem = "Preencha título, descrição e categoria."
                });
            }

            var precoTexto = Preco
                .Replace("R$", "")
                .Replace(" ", "")
                .Replace(".", "")
                .Replace(",", ".");

            if (!decimal.TryParse(
                    precoTexto,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var preco) ||
                preco <= 0)
            {
                return BadRequest(new
                {
                    mensagem = "Informe um preço válido."
                });
            }

            var produto = await _produtosApi.PublicarAsync(
                Titulo,
                Descricao,
                Categoria,
                preco,
                Imagem,
                Video);

            if (produto == null)
            {
                return StatusCode(502, new
                {
                    mensagem = "A API não conseguiu cadastrar o produto. Verifique a configuração e tente novamente."
                });
            }

            return Ok(new
            {
                mensagem = "Produto publicado com sucesso!",
                id = produto.Id
            });
        }
    }
}
