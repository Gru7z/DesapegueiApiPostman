using Desapeguei.Api.Data;
using Desapeguei.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Api.Repositories
{
    public class ProdutoRepository
    {
        private readonly ProdutoContext _context;

        public ProdutoRepository(ProdutoContext context)
        {
            _context = context;
        }

        // Uso público: só produtos disponíveis (os vendidos ficam de fora)
        public async Task<List<Produto>> GetAllAsync(string? categoria)
        {
            var query = _context.Produtos
                .Where(p => p.Disponivel)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(p =>
                    p.Categoria.ToLower() == categoria.ToLower());
            }

            return await query
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<Produto?> GetByIdAsync(Guid id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
        }

        public void Delete(Produto produto)
        {
            _context.Produtos.Remove(produto);
        }

        // Verdadeiro se o produto já tem uma venda (não pode editar nem apagar)
        public async Task<bool> FoiVendidoAsync(Guid produtoId)
        {
            return await _context.Vendas.AnyAsync(v => v.ProdutoId == produtoId);
        }

        // Verdadeiro se o produto foi comprado por alguém que está desativado
        public async Task<bool> CompradorDesativadoAsync(Guid produtoId)
        {
            return await _context.Vendas
                .AnyAsync(v => v.ProdutoId == produtoId && !v.Comprador!.Ativo);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}