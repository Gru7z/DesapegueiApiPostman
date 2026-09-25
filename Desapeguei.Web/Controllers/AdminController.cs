using Desapeguei.Web.Models;
using Desapeguei.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Web.Controllers
{
    // Painel exclusivo do administrador: só quem estiver no papel "Admin"
    // consegue ver, editar ou apagar qualquer produto cadastrado no site.
    //
    // Antes esse controller acessava o banco de produtos direto (EF).
    // Agora os produtos vivem na Desapeguei.Api, então tudo aqui passa
    // pelo ProdutosApiClient (chamadas HTTP para a API).
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProdutosApiClient _api;

        public AdminController(ProdutosApiClient api)
        {
            _api = api;
        }

        // GET /Admin
        // Lista TODOS os produtos cadastrados (inclusive os indisponíveis/ocultos)
        public async Task<IActionResult> Index(string? busca)
        {
            ViewData["TituloSite"] = "PAINEL ADMIN";

            var produtos = await _api.ListarTodosAsync(busca);

            ViewData["Busca"] = busca;
            ViewData["TotalProdutos"] = produtos.Count;

            return View(produtos);
        }

        // GET /Admin/Editar/{id}
        public async Task<IActionResult> Editar(Guid id)
        {
            ViewData["TituloSite"] = "EDITAR PRODUTO";

            var produto = await _api.ObterPorIdAsync(id);
            if (produto == null) return NotFound();

            return View(produto);
        }

        // POST /Admin/Editar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Guid id, Produto produtoEditado, IFormFile? novaImagem)
        {
            if (id != produtoEditado.Id) return NotFound();

            // Remove a validação de campos que não fazem parte do formulário de edição
            ModelState.Remove(nameof(Produto.CriadoPor));
            ModelState.Remove(nameof(Produto.ImagemUrl));

            if (!ModelState.IsValid)
            {
                ViewData["TituloSite"] = "EDITAR PRODUTO";
                return View(produtoEditado);
            }

            var atualizado = await _api.AtualizarAsync(id, produtoEditado, novaImagem);

            if (atualizado == null)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível atualizar o produto na API.");
                ViewData["TituloSite"] = "EDITAR PRODUTO";
                return View(produtoEditado);
            }

            TempData["MensagemAdmin"] = $"Produto \"{atualizado.Titulo}\" atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Apagar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apagar(Guid id)
        {
            var produto = await _api.ObterPorIdAsync(id);

            await _api.ApagarAsync(id);

            if (produto != null)
                TempData["MensagemAdmin"] = $"Produto \"{produto.Titulo}\" apagado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/AlternarDisponibilidade/{id}
        // Oculta/exibe o produto no site sem precisar apagar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlternarDisponibilidade(Guid id)
        {
            await _api.AlternarDisponibilidadeAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
