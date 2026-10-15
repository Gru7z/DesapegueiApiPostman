namespace Desapeguei.Api.Models
{
    public class Venda
    {
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        public Guid CompradorId { get; set; }
        public Usuario? Comprador { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.UtcNow;
    }
}