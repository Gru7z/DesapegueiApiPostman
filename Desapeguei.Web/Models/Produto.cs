using System.ComponentModel.DataAnnotations;

namespace Desapeguei.Web.Models
{
    // Não é mais uma entidade do EF: é só o "molde" usado para exibir
    // e editar produtos nas telas do site. Os dados reais de produto
    // vivem na Desapeguei.Api (banco de produtos) e chegam até aqui
    // via HTTP, através do ProdutosApiClient.
    public class Produto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Informe um título para o anúncio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escreva uma descrição para o produto.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 1000 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione uma categoria.")]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o preço.")]
        [Range(0.01, 999999, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        public string? ImagemUrl { get; set; }

        public string? CaminhoVideo { get; set; }

        public string? CriadoPor { get; set; }

        public DateTime DataCriacao { get; set; }

        public bool Disponivel { get; set; } = true;
    }
}
