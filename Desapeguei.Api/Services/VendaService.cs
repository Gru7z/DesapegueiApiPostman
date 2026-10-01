using Desapeguei.Api.Models;
using Desapeguei.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Api.Services
{
    public class VendaService
    {
        private readonly VendaRepository _vendaRepository;
        private readonly ProdutoRepository _produtoRepository;
        private readonly IConfiguration _config;

        public VendaService(VendaRepository vendaRepository,
                            ProdutoRepository produtoRepository,
                            IConfiguration config)
        {
            _vendaRepository = vendaRepository;
            _produtoRepository = produtoRepository;
            _config = config;
        }

        // Devolve null se o produto não existe
        public async Task<Venda?> ComprarAsync(Guid produtoId, Guid compradorId)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null) return null;

            if (!produto.Disponivel)
                throw new ArgumentException("Este produto não está disponível.");

            var venda = new Venda
            {
                ProdutoId = produto.Id,
                CompradorId = compradorId,
                Valor = produto.Preco,
                DataVenda = DateTime.UtcNow
            };

            produto.Disponivel = false; // fica indisponível direto
            await _vendaRepository.AddAsync(venda);

            try
            {
                await _vendaRepository.SaveChangesAsync(); // grava venda + produto juntos
            }
            catch (DbUpdateException)
            {
                throw new ArgumentException("Este produto acabou de ser vendido.");
            }

            return await _vendaRepository.GetByIdAsync(venda.Id); // já com os dados do comprador
        }

        public async Task<List<Venda>> MinhasComprasAsync(Guid compradorId)
        {
            return await _vendaRepository.GetByCompradorAsync(compradorId);
        }

        // Só o admin vê todas as vendas e quem comprou
        public async Task<List<Venda>> ListarTodasAsync(string? emailSolicitante)
        {
            if (!string.Equals(emailSolicitante, _config["AdminEmail"], StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException();

            return await _vendaRepository.GetAllAsync();
        }
    }
}