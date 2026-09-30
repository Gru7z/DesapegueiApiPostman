using Desapeguei.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Api.Data
{
    public class ProdutoContext(DbContextOptions<ProdutoContext> options) : DbContext(options)
    {
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique(); // não deixa repetir email
                e.Property(u => u.Nome).HasMaxLength(100).IsRequired();
                e.Property(u => u.Email).HasMaxLength(150).IsRequired();
                e.Property(u => u.Telefone).HasMaxLength(20).IsRequired();
                e.Property(u => u.Senha).IsRequired();
            });
        }
    }
}