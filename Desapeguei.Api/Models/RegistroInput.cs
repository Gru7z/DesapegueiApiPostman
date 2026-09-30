using System.ComponentModel.DataAnnotations;

namespace Desapeguei.Api.Models
{
    public class RegistroInput
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Senha { get; set; } = string.Empty;
    }
}