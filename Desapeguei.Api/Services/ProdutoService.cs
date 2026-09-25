using Desapeguei.Api.Models;
using Desapeguei.Api.Repositories;

namespace Desapeguei.Api.Services
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _repository;

        public ProdutoService(ProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Produto>> GetAllAsync(string? categoria)
        {
            return await _repository.GetAllAsync(categoria);
        }

        public async Task<List<Produto>> GetAllAdminAsync(string? busca)
        {
            return await _repository.GetAllAdminAsync(busca);
        }

        public async Task<Produto?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Produto> CreateAsync(ProdutoInput input)
        {
            Validar(input);

            var produto = new Produto
            {
                Titulo = input.Titulo.Trim(),
                Descricao = input.Descricao.Trim(),
                Categoria = input.Categoria.Trim(),
                Preco = input.Preco,
                DataCriacao = DateTime.UtcNow,
                Disponivel = input.Disponivel ?? true
            };

            await _repository.AddAsync(produto);
            await _repository.SaveChangesAsync();

            return produto;
        }

        public async Task<Produto?> UpdateAsync(Guid id, ProdutoInput input)
        {
            var produto = await _repository.GetByIdAsync(id);
            if (produto == null) return null;

            Validar(input);

            produto.Titulo = input.Titulo.Trim();
            produto.Descricao = input.Descricao.Trim();
            produto.Categoria = input.Categoria.Trim();
            produto.Preco = input.Preco;

            if (input.Disponivel.HasValue)
                produto.Disponivel = input.Disponivel.Value;

            await _repository.SaveChangesAsync();

            return produto;
        }

        public async Task<Produto?> AlternarDisponibilidadeAsync(Guid id)
        {
            var produto = await _repository.GetByIdAsync(id);
            if (produto == null) return null;

            produto.Disponivel = !produto.Disponivel;
            await _repository.SaveChangesAsync();

            return produto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var produto = await _repository.GetByIdAsync(id);
            if (produto == null) return false;

            _repository.Delete(produto);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static void Validar(ProdutoInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Titulo))
                throw new ArgumentException("O título é obrigatório.");

            if (string.IsNullOrWhiteSpace(input.Descricao))
                throw new ArgumentException("A descrição é obrigatória.");

            if (string.IsNullOrWhiteSpace(input.Categoria))
                throw new ArgumentException("A categoria é obrigatória.");

            if (input.Preco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.");
        }
    }
}
