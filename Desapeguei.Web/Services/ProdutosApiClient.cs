using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Desapeguei.Web.Models;

namespace Desapeguei.Web.Services
{
    // Fala com a Desapeguei.Api (endpoints em /api/produtos) no lugar
    // do antigo acesso direto ao banco. É usado pelo painel Admin.
    public class ProdutosApiClient
    {
        private readonly HttpClient _http;

        private static readonly JsonSerializerOptions _json =
            new(JsonSerializerDefaults.Web);

        public ProdutosApiClient(HttpClient http)
        {
            _http = http;
        }

        // GET /api/produtos/admin?busca=...  -> traz TODOS os produtos,
        // inclusive os indisponíveis/ocultos (só o Admin usa isso).
        public async Task<List<Produto>> ListarTodosAsync(string? busca)
        {
            var url = "api/produtos/admin";
            if (!string.IsNullOrWhiteSpace(busca))
                url += $"?busca={Uri.EscapeDataString(busca)}";

            var resposta = await _http.GetAsync(url);
            if (!resposta.IsSuccessStatusCode)
                return new List<Produto>();

            var stream = await resposta.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Produto>>(stream, _json)
                ?? new List<Produto>();
        }

        public async Task<Produto?> ObterPorIdAsync(Guid id)
        {
            var resposta = await _http.GetAsync($"api/produtos/{id}");
            if (!resposta.IsSuccessStatusCode)
                return null;

            var stream = await resposta.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Produto>(stream, _json);
        }

        // PUT /api/produtos/{id}/upload (multipart) -> usado pela tela
        // de edição do Admin, que pode enviar uma nova imagem.
        public async Task<Produto?> AtualizarAsync(Guid id, Produto produto, IFormFile? novaImagem)
        {
            using var form = new MultipartFormDataContent
            {
                { new StringContent(produto.Titulo), "Titulo" },
                { new StringContent(produto.Descricao), "Descricao" },
                { new StringContent(produto.Categoria), "Categoria" },
                { new StringContent(produto.Preco.ToString(CultureInfo.InvariantCulture)), "Preco" },
                { new StringContent(produto.Disponivel.ToString()), "Disponivel" }
            };

            if (novaImagem != null && novaImagem.Length > 0)
            {
                var conteudoImagem = new StreamContent(novaImagem.OpenReadStream());
                conteudoImagem.Headers.ContentType =
                    new MediaTypeHeaderValue(novaImagem.ContentType);

                form.Add(conteudoImagem, "Imagem", novaImagem.FileName);
            }

            var resposta = await _http.PutAsync($"api/produtos/{id}/upload", form);
            if (!resposta.IsSuccessStatusCode)
                return null;

            var stream = await resposta.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Produto>(stream, _json);
        }

        public async Task ApagarAsync(Guid id)
        {
            await _http.DeleteAsync($"api/produtos/{id}");
        }

        public async Task AlternarDisponibilidadeAsync(Guid id)
        {
            await _http.PatchAsync($"api/produtos/{id}/disponibilidade", content: null);
        }
    }
}
