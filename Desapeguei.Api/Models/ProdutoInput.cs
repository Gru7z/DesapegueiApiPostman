namespace Desapeguei.Api.Models
{
    // DTO simples, só com JSON (sem arquivo) — usado pelos endpoints
    // "clássicos" (POST /api/produtos, PUT /api/produtos/{id}),
    // pensados pra serem testados fácil no Postman.
    public class ProdutoInput
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public bool? Disponivel { get; set; }
    }
}
