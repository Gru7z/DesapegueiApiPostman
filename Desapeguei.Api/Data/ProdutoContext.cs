using Desapeguei.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Api.Data
{
    // Este é o "banco de produtos": só a tabela Produtos.
    // Não conhece usuários/Identity — isso pertence à Desapeguei.Web.
    public class ProdutoContext(DbContextOptions<ProdutoContext> options) : DbContext(options)
    {
        public DbSet<Produto> Produtos => Set<Produto>();
    }
}
