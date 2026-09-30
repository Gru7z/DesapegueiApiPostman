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
            var token = await _service.LoginAsync(input);

            if (token == null)
                return Unauthorized(new { mensagem = "Email ou senha inválidos." });

            return Ok(new { token });
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

        // DELETE /api/auth/usuarios/{id}  (só o admin)
        [Authorize]
        [HttpDelete("usuarios/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var emailLogado = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                var removido = await _service.DeleteAsync(id, emailLogado);

                if (!removido) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}