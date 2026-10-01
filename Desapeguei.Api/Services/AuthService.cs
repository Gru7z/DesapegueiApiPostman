using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Desapeguei.Api.Models;
using Desapeguei.Api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Desapeguei.Api.Services
{
    public class AuthService
    {
        private readonly UsuarioRepository _repository;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public AuthService(UsuarioRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        public async Task<Usuario> RegistrarAsync(RegistroInput input)
        {
            var email = input.Email.Trim().ToLowerInvariant();

            if (await _repository.GetByEmailAsync(email) != null)
                throw new ArgumentException("Email já cadastrado.");

            var usuario = new Usuario
            {
                Nome = input.Nome.Trim(),
                Email = email,
                Telefone = input.Telefone.Trim(),
                Admin = false // quem se registra pela API nunca é admin
            };

            usuario.Senha = _hasher.HashPassword(usuario, input.Senha);

            await _repository.AddAsync(usuario);
            await _repository.SaveChangesAsync();

            return usuario;
        }

        // Devolve o token, ou null se o usuário não existe / senha errada
        public async Task<string?> LoginAsync(LoginInput input)
        {
            var email = input.Email.Trim().ToLowerInvariant();
            var usuario = await _repository.GetByEmailAsync(email);

            if (usuario == null)
                return null;

            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.Senha, input.Senha);

            if (resultado == PasswordVerificationResult.Failed)
                return null;

            // Conta desativada não recebe token
            if (!usuario.Ativo)
                throw new UnauthorizedAccessException("Conta desativada.");

            return GerarToken(usuario);
        }

        private string GerarToken(Usuario usuario)
        {
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var minutos = _config.GetValue<int>("Jwt:ExpiraEmMinutos", 120);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutos),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Alterna entre ativo e desativado (o controller já garante que é admin)
        public async Task<Usuario?> AlternarAtivoAsync(Guid id, Guid idSolicitante)
        {
            if (id == idSolicitante)
                throw new ArgumentException("O admin não pode desativar a própria conta.");

            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return null;

            usuario.Ativo = !usuario.Ativo;
            await _repository.SaveChangesAsync();

            return usuario;
        }

        // Lista todos, inclusive os desativados
        public async Task<List<Usuario>> ListarAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}