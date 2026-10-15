using Desapeguei.Api.Data;
using Desapeguei.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Api.Repositories
{
    public class VendaRepository
    {
        private readonly ProdutoContext _context;

        public VendaRepository(ProdutoContext context)
        {
            _context = context;
        }

        public async Task<Venda?> GetByIdAsync(Guid id)
        {
            return await _context.Vendas
                .Include(v => v.Produto)
                .Include(v => v.Comprador)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Venda>> GetAllAsync()
        {
            return await _context.Vendas
                .Include(v => v.Produto)
                .Include(v => v.Comprador)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        public async Task<List<Venda>> GetByCompradorAsync(Guid compradorId)
        {
            return await _context.Vendas
                .Include(v => v.Produto)
                .Include(v => v.Comprador)
                .Where(v => v.CompradorId == compradorId)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        public async Task AddAsync(Venda venda)
        {
            await _context.Vendas.AddAsync(venda);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}