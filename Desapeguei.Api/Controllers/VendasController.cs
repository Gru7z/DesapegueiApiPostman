using System.Security.Claims;
using Desapeguei.Api.Models;
using Desapeguei.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/vendas")]
    public class VendasController : ControllerBase
    {
        private readonly VendaService _service;

        public VendasController(VendaService service)
        {
            _service = service;
        }

        // POST /api/vendas   { "produtoId": "..." }
        [HttpPost]
        public async Task<IActionResult> Comprar([FromBody] VendaInput input)
        {
            var compradorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var venda = await _service.ComprarAsync(input.ProdutoId, compradorId);

                if (venda == null)
                    return NotFound(new { mensagem = "Produto não encontrado." });

                return Created("", ParaResposta(venda));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // GET /api/vendas/minhas
        [HttpGet("minhas")]
        public async Task<IActionResult> MinhasCompras()
        {
            var compradorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var compras = await _service.MinhasComprasAsync(compradorId);
            return Ok(compras.Select(ParaResposta));
        }

        // GET /api/vendas   (só admin: todas as vendas e quem comprou)
        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var emailLogado = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                var vendas = await _service.ListarTodasAsync(emailLogado);
                return Ok(vendas.Select(ParaResposta));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Monta a resposta sem expor a senha do comprador
        private static object ParaResposta(Venda v) => new
        {
            v.Id,
            v.Valor,
            v.DataVenda,
            Produto = v.Produto == null ? null : new { v.Produto.Id, v.Produto.Titulo },
            Comprador = v.Comprador == null ? null : new { v.Comprador.Id, v.Comprador.Nome, v.Comprador.Email, v.Comprador.Ativo }
        };
    }
}