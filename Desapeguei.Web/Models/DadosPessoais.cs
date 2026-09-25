// Models\DadosPessoais.cs
using System.ComponentModel.DataAnnotations;

namespace Desapeguei.Web.Models
{
    public class DadosPessoais
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public string Nome { get; set; } = "";

        public string Telefone { get; set; } = "";

        public string Endereco { get; set; } = "";
    }
}