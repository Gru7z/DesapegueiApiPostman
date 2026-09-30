using Desapeguei.Api.Models;
using Desapeguei.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Api.Controllers
{
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
        // PÚBLICO: só produtos disponíveis.
        [HttpGet]
        public async Task<ActionResult<List<Produto>>> GetAll([FromQuery] string? categoria)
        {
            var produtos = await _service.GetAllAsync(categoria);
            return Ok(produtos);
        }

        // GET /api/produtos/{id}
        // PÚBLICO
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetById(Guid id)
        {
            var produto = await _service.GetByIdAsync(id);

            if (produto == null)
                return NotFound(new { mensagem = $"Produto com Id '{id}' não foi encontrado." });

            return Ok(produto);
        }

        // POST /api/produtos
        // PROTEGIDO: precisa de token
        [Authorize]
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
        // PROTEGIDO: precisa de token
        [Authorize]
        [HttpPut("{id}")]
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

        // DELETE /api/produtos/{id}
        // PROTEGIDO: precisa de token
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var removido = await _service.DeleteAsync(id);

            if (!removido)
                return NotFound(new { mensagem = $"Produto com Id '{id}' não foi encontrado." });

            return NoContent();
        }
    }
}