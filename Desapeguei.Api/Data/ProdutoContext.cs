using Desapeguei.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Api.Data
{
    public class ProdutoContext(DbContextOptions<ProdutoContext> options) : DbContext(options)
    {
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Venda> Vendas => Set<Venda>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Nome).HasMaxLength(100).IsRequired();
                e.Property(u => u.Email).HasMaxLength(150).IsRequired();
                e.Property(u => u.Telefone).HasMaxLength(20).IsRequired();
                e.Property(u => u.Senha).IsRequired();
                e.Property(u => u.Ativo).HasDefaultValue(true); // usuários que já existem ficam ativos
            });

            modelBuilder.Entity<Venda>(e =>
            {
                e.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                e.HasIndex(v => v.ProdutoId).IsUnique(); // um produto só tem uma venda

                e.HasOne(v => v.Produto).WithMany()
                    .HasForeignKey(v => v.ProdutoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(v => v.Comprador).WithMany()
                    .HasForeignKey(v => v.CompradorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}