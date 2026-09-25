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

        // Uso público (Postman / site) — só produtos disponíveis
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

        // Uso do Admin — todos os produtos, inclusive os ocultos
        public async Task<List<Produto>> GetAllAdminAsync(string? busca)
        {
            var query = _context.Produtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.ToLower();
                query = query.Where(p =>
                    p.Titulo.ToLower().Contains(termo) ||
                    p.Categoria.ToLower().Contains(termo));
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
