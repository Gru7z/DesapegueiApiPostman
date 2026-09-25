using Desapeguei.Api.Models;
using Desapeguei.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Api.Controllers
{
    // API simples para testar no Postman: só JSON, sem autenticação e sem upload de mídia.
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutosController(ProdutoService service)
        {
            _service = service;
        }

        // GET /api/produtos
        // GET /api/produtos?categoria=Moveis
        // Só produtos disponíveis.
        [HttpGet]
        public async Task<ActionResult<List<Produto>>> GetAll([FromQuery] string? categoria)
        {
            var produtos = await _service.GetAllAsync(categoria);
            return Ok(produtos);
        }

        // GET /api/produtos/admin?busca=...
        // Todos os produtos, inclusive os ocultos.
        [HttpGet("admin")]
        public async Task<ActionResult<List<Produto>>> GetAllAdmin([FromQuery] string? busca)
        {
            var produtos = await _service.GetAllAdminAsync(busca);
            return Ok(produtos);
        }

        // GET /api/produtos/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Produto>> GetById(Guid id)
        {
            var produto = await _service.GetByIdAsync(id);

            if (produto == null)
                return NotFound(new { mensagem = $"Produto com Id '{id}' não foi encontrado." });

            return Ok(produto);
        }

        // POST /api/produtos
        // Body raw/JSON — pensado pra testar fácil no Postman.
        [HttpPost]
        public async Task<ActionResult<Produto>> Create([FromBody] ProdutoInput input)
        {
            try
            {
                var produto = await _service.CreateAsync(input);
                return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // PUT /api/produtos/{id}
        // Body raw/JSON.
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Produto>> Update(Guid id, [FromBody] ProdutoInput input)
        {
            try
            {
                var produto = await _service.UpdateAsync(id, input);

                if (produto == null)
                    return NotFound(new { mensagem = $"Produto com Id '{id}' não foi encontrado." });

                return Ok(produto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // PATCH /api/produtos/{id}/disponibilidade
        // Oculta/reexibe o produto sem apagar.
        [HttpPatch("{id:guid}/disponibilidade")]
        public async Task<ActionResult<Produto>> AlternarDisponibilidade(Guid id)
        {
            var produto = await _service.AlternarDisponibilidadeAsync(id);

            if (produto == null)
                return NotFound(new { mensagem = $"Produto com Id '{id}' não foi encontrado." });

            return Ok(produto);
        }

        // DELETE /api/produtos/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var removido = await _service.DeleteAsync(id);

            if (!removido)
                return NotFound(new { mensagem = $"Produto com Id '{id}' não foi encontrado." });

            return NoContent();
        }
    }
}
