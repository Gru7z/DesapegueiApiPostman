using System.Security.Claims;
using Desapeguei.Api.Models;
using Desapeguei.Api.Repositories;
using Desapeguei.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desapeguei.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;
        private readonly UsuarioRepository _repository;
        private readonly IConfiguration _config;

        public AuthController(AuthService service, UsuarioRepository repository, IConfiguration config)
        {
            _service = service;
            _repository = repository;
            _config = config;
        }

        // GET /api/auth/usuarios  (precisa do token)
        [Authorize]
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _repository.GetAllAsync();

            // Nunca devolva a Senha: só os campos abaixo
            var resultado = usuarios.Select(u => new { u.Id, u.Nome, u.Email, u.Telefone });

            return Ok(resultado);
        }

        // POST /api/auth/registro
        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroInput input)
        {
            try
            {
                var usuario = await _service.RegistrarAsync(input);
                return Created("", new { usuario.Id, usuario.Nome, usuario.Email, usuario.Telefone });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input)
        {
            try
            {
                var token = await _service.LoginAsync(input);

                if (token == null)
                    return Unauthorized(new { mensagem = "Email ou senha inválidos." });

                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { mensagem = ex.Message });
            }
        }

        // GET /api/auth/me  (precisa do token)
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var usuario = await _repository.GetByIdAsync(id);

            if (usuario == null) return NotFound();

            return Ok(new { usuario.Id, usuario.Nome, usuario.Email, usuario.Telefone });
        }

        // GET /api/auth/usuarios  (só admin) - mostra também os desativados
        [Authorize]
        [HttpGet("desativados")]
        public async Task<IActionResult> Listar()
        {
            var emailLogado = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                var usuarios = await _service.ListarAsync(emailLogado);
                return Ok(usuarios.Select(u => new { u.Id, u.Nome, u.Email, u.Telefone, u.Ativo }));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // PATCH /api/auth/usuarios/{id}/status  (só admin) - alterna ativo/desativado
        [Authorize]
        [HttpPut("usuarios/{id}/status")]
        public async Task<IActionResult> AlternarStatus(Guid id)
        {
            var idLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var emailLogado = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                var usuario = await _service.AlternarAtivoAsync(id, idLogado, emailLogado);

                if (usuario == null)
                    return NotFound(new { mensagem = "Usuário não encontrado." });

                return Ok(new { usuario.Id, usuario.Nome, usuario.Email, usuario.Ativo });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }


    }
}