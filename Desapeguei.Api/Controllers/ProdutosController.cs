using Desapeguei.Api.Models;
using Desapeguei.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Desapeguei.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ProdutoService _service;

        public ProdutosController(
    ProdutoService service,
    IConfiguration configuration,
    IWebHostEnvironment environment)
        {
            _service = service;
            _configuration = configuration;
            _environment = environment;
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

        [HttpPost("upload")]
        [RequestSizeLimit(110_000_000)]
        public async Task<IActionResult> Upload(
            [FromHeader(Name = "X-Internal-Key")] string chave,
            [FromForm] string Titulo,
            [FromForm] string Descricao,
            [FromForm] string Categoria,
            [FromForm] string Preco,
            [FromForm] IFormFile? Imagem,
            [FromForm] IFormFile? Video)
        {
            var chaveConfigurada = _configuration["InternalApiKey"];

            if (string.IsNullOrWhiteSpace(chaveConfigurada) ||
                string.IsNullOrWhiteSpace(chave) ||
                !System.Security.Cryptography.CryptographicOperations
                    .FixedTimeEquals(
                        System.Text.Encoding.UTF8.GetBytes(chave),
                        System.Text.Encoding.UTF8.GetBytes(chaveConfigurada)))
            {
                return Unauthorized(new
                {
                    mensagem = "Chave interna inválida."
                });
            }

            if (string.IsNullOrWhiteSpace(Titulo) ||
                string.IsNullOrWhiteSpace(Descricao) ||
                string.IsNullOrWhiteSpace(Categoria))
            {
                return BadRequest(new
                {
                    mensagem = "Título, descrição e categoria são obrigatórios."
                });
            }

            var precoTexto = Preco
                .Replace("R$", "")
                .Replace(" ", "")
                .Replace(".", "")
                .Replace(",", ".");

            if (!decimal.TryParse(
                    precoTexto,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var preco) ||
                preco <= 0)
            {
                return BadRequest(new
                {
                    mensagem = "Preço inválido."
                });
            }

            if (Imagem != null &&
                (!Imagem.ContentType.StartsWith(
                    "image/", StringComparison.OrdinalIgnoreCase) ||
                 Imagem.Length > 10 * 1024 * 1024))
            {
                return BadRequest(new
                {
                    mensagem = "A imagem deve ser válida e ter até 10 MB."
                });
            }

            if (Video != null &&
                (!Video.ContentType.StartsWith(
                    "video/", StringComparison.OrdinalIgnoreCase) ||
                 Video.Length > 100 * 1024 * 1024))
            {
                return BadRequest(new
                {
                    mensagem = "O vídeo deve ser válido e ter até 100 MB."
                });
            }

            var pastaUploads = Path.Combine(
                _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
                "uploads");

            Directory.CreateDirectory(pastaUploads);

            string? imagemUrl = null;
            string? videoUrl = null;

            if (Imagem != null && Imagem.Length > 0)
            {
                var extensao = Path.GetExtension(Imagem.FileName);
                var nome = $"{Guid.NewGuid():N}{extensao}";

                var caminho = Path.Combine(pastaUploads, nome);

                await using var stream = System.IO.File.Create(caminho);
                await Imagem.CopyToAsync(stream);

                imagemUrl = $"/uploads/{nome}";
            }

            if (Video != null && Video.Length > 0)
            {
                var extensao = Path.GetExtension(Video.FileName);
                var nome = $"{Guid.NewGuid():N}{extensao}";

                var caminho = Path.Combine(pastaUploads, nome);

                await using var stream = System.IO.File.Create(caminho);
                await Video.CopyToAsync(stream);

                videoUrl = $"/uploads/{nome}";
            }

            try
            {
                var produto = await _service.CreateAsync(new ProdutoInput
                {
                    Titulo = Titulo,
                    Descricao = Descricao,
                    Categoria = Categoria,
                    Preco = preco,
                    Disponivel = true
                });

                produto.ImagemUrl = imagemUrl;
                produto.CaminhoVideo = videoUrl;

                // Atualiza as URLs dos arquivos no registro recém-criado.
                // O serviço CreateAsync já salvou o produto; por isso,
                // essa atualização precisa ser persistida pelo repositório.
                await _service.SalvarArquivosAsync(produto);

                return Ok(produto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // =========================================================
        // LISTAR TODOS - ADMIN
        // =========================================================

        [HttpGet("admin")]
        public async Task<ActionResult<List<Produto>>> GetAllAdmin(
            [FromQuery] string? busca)
        {
            if (!ValidarChaveAdministrativa())
                return Unauthorized(new
                {
                    mensagem = "Acesso administrativo não autorizado."
                });

            var produtos = await _service.GetAllAdminAsync(busca);

            return Ok(produtos);
        }
        // =========================================================
        // EDITAR - ADMIN
        // =========================================================

        [HttpPut("{id}")]
        public async Task<ActionResult<Produto>> Update(
            Guid id,
            [FromForm] ProdutoInput input,
            IFormFile? Imagem)
        {
            if (!ValidarChaveAdministrativa())
                return Unauthorized(new
                {
                    mensagem = "Acesso administrativo não autorizado."
                });

            try
            {
                var produto = await _service.UpdateAsync(
                    id,
                    input,
                    Imagem);

                if (produto == null)
                {
                    return NotFound(new
                    {
                        mensagem = "Produto não encontrado."
                    });
                }

                return Ok(produto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }
        // =========================================================
        // EXCLUIR - ADMIN
        // =========================================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ValidarChaveAdministrativa())
                return Unauthorized(new
                {
                    mensagem = "Acesso administrativo não autorizado."
                });

            try
            {
                var removido = await _service.DeleteAsync(id);

                if (!removido)
                {
                    return NotFound(new
                    {
                        mensagem = "Produto não encontrado."
                    });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }


        // =========================================================
        // OCULTAR / REATIVAR - ADMIN
        // =========================================================

        [HttpPatch("{id}/disponibilidade")]
        public async Task<ActionResult<Produto>> AlternarDisponibilidade(Guid id)
        {
            if (!ValidarChaveAdministrativa())
                return Unauthorized(new
                {
                    mensagem = "Acesso administrativo não autorizado."
                });

            try
            {
                var produto =
                    await _service.AlternarDisponibilidadeAsync(id);

                if (produto == null)
                {
                    return NotFound(new
                    {
                        mensagem = "Produto não encontrado."
                    });
                }

                return Ok(produto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }
        // =========================================================
        // CHAVE INTERNA
        // =========================================================

        private bool ValidarChaveAdministrativa()
        {
            var chaveRecebida =
                Request.Headers["X-Internal-Api-Key"]
                    .FirstOrDefault();

            var chaveCorreta =
                _configuration["InternalApiKey"];

            if (string.IsNullOrWhiteSpace(chaveRecebida))
                return false;

            if (string.IsNullOrWhiteSpace(chaveCorreta))
                return false;

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(chaveRecebida),
                Encoding.UTF8.GetBytes(chaveCorreta));
        }

    }
}