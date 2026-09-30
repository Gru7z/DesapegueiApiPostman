using System.ComponentModel.DataAnnotations;

namespace Desapeguei.Api.Models
{
    public class LoginInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;
    }
}