using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Desapeguei.Web.Models;

namespace Desapeguei.Web.Services
{
    public class ProdutosApiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        private static readonly JsonSerializerOptions _json =
            new(JsonSerializerDefaults.Web);

        public ProdutosApiClient(
            HttpClient http,
            IConfiguration configuration)
        {
            _http = http;

            _apiKey = configuration["InternalApiKey"]
                ?? throw new InvalidOperationException(
                    "InternalApiKey não configurada.");
        }

        // =========================================================
        // CHAVE INTERNA
        // =========================================================

        private void AdicionarChaveInterna()
        {
            _http.DefaultRequestHeaders.Remove(
                "X-Internal-Api-Key");

            _http.DefaultRequestHeaders.Add(
                "X-Internal-Api-Key",
                _apiKey);
        }

        // =========================================================
        // LISTAR TODOS - ADMIN
        // =========================================================

        public async Task<List<Produto>> ListarTodosAsync(
            string? busca)
        {
            AdicionarChaveInterna();

            var url = "api/produtos/admin";

            if (!string.IsNullOrWhiteSpace(busca))
            {
                url +=
                    $"?busca={Uri.EscapeDataString(busca)}";
            }

            var resposta =
                await _http.GetAsync(url);

            if (!resposta.IsSuccessStatusCode)
            {
                return new List<Produto>();
            }

            var stream =
                await resposta.Content.ReadAsStreamAsync();

            return await JsonSerializer
                .DeserializeAsync<List<Produto>>(
                    stream,
                    _json)
                ?? new List<Produto>();
        }

        // =========================================================
        // BUSCAR PRODUTO
        // =========================================================

        public async Task<Produto?> ObterPorIdAsync(
            Guid id)
        {
            // Para o Admin, também mandamos a chave.
            AdicionarChaveInterna();

            var resposta =
                await _http.GetAsync(
                    $"api/produtos/{id}");

            if (!resposta.IsSuccessStatusCode)
            {
                return null;
            }

            var stream =
                await resposta.Content.ReadAsStreamAsync();

            return await JsonSerializer
                .DeserializeAsync<Produto>(
                    stream,
                    _json);
        }

        // =========================================================
        // EDITAR PRODUTO - ADMIN
        // =========================================================

        public async Task<(bool Sucesso, string? Erro, Produto? Produto)>
            AtualizarAsync(
                Guid id,
                Produto produto,
                IFormFile? novaImagem)
        {
            AdicionarChaveInterna();

            using var form =
                new MultipartFormDataContent();

            form.Add(
                new StringContent(
                    produto.Titulo ?? ""),
                "Titulo");

            form.Add(
                new StringContent(
                    produto.Descricao ?? ""),
                "Descricao");

            form.Add(
                new StringContent(
                    produto.Categoria ?? ""),
                "Categoria");

            form.Add(
                new StringContent(
                    produto.Preco.ToString(
                        CultureInfo.InvariantCulture)),
                "Preco");

            form.Add(
                new StringContent(
                    produto.Disponivel
                        ? "true"
                        : "false"),
                "Disponivel");

            // -----------------------------------------------------
            // NOVA IMAGEM
            // -----------------------------------------------------

            if (novaImagem != null &&
                novaImagem.Length > 0)
            {
                var arquivo =
                    new StreamContent(
                        novaImagem.OpenReadStream());

                if (!string.IsNullOrWhiteSpace(
                    novaImagem.ContentType))
                {
                    arquivo.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            novaImagem.ContentType);
                }

                form.Add(
                    arquivo,
                    "Imagem",
                    novaImagem.FileName);
            }

            // IMPORTANTE:
            // A rota agora é /api/produtos/{id}
            var resposta =
                await _http.PutAsync(
                    $"api/produtos/{id}",
                    form);

            if (!resposta.IsSuccessStatusCode)
            {
                var erro =
                    await resposta.Content.ReadAsStringAsync();

                return (
                    false,
                    string.IsNullOrWhiteSpace(erro)
                        ? $"Erro HTTP {(int)resposta.StatusCode}"
                        : erro,
                    null);
            }

            var stream =
                await resposta.Content.ReadAsStreamAsync();

            var produtoAtualizado =
                await JsonSerializer
                    .DeserializeAsync<Produto>(
                        stream,
                        _json);

            return (
                true,
                null,
                produtoAtualizado);
        }

        // =========================================================
        // EXCLUIR - ADMIN
        // =========================================================

        public async Task<(bool Sucesso, string? Erro)>
            ApagarAsync(Guid id)
        {
            AdicionarChaveInterna();

            var resposta =
                await _http.DeleteAsync(
                    $"api/produtos/{id}");

            if (resposta.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var erro =
                await resposta.Content.ReadAsStringAsync();

            return (
                false,
                string.IsNullOrWhiteSpace(erro)
                    ? $"Erro HTTP {(int)resposta.StatusCode}"
                    : erro);
        }

        // =========================================================
        // OCULTAR / REATIVAR - ADMIN
        // =========================================================

        public async Task<(bool Sucesso, string? Erro)>
            AlternarDisponibilidadeAsync(Guid id)
        {
            AdicionarChaveInterna();

            var resposta =
                await _http.PatchAsync(
                    $"api/produtos/{id}/disponibilidade",
                    content: null);

            if (resposta.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var erro =
                await resposta.Content.ReadAsStringAsync();

            return (
                false,
                string.IsNullOrWhiteSpace(erro)
                    ? $"Erro HTTP {(int)resposta.StatusCode}"
                    : erro);
        }

        // =========================================================
        // PUBLICAR PRODUTO
        // =========================================================

        public async Task<Produto?> PublicarAsync(
            string titulo,
            string descricao,
            string categoria,
            decimal preco,
            IFormFile? imagem,
            IFormFile? video)
        {
            using var form =
                new MultipartFormDataContent();

            form.Add(
                new StringContent(titulo),
                "Titulo");

            form.Add(
                new StringContent(descricao),
                "Descricao");

            form.Add(
                new StringContent(categoria),
                "Categoria");

            form.Add(
                new StringContent(
                    preco.ToString(
                        CultureInfo.InvariantCulture)),
                "Preco");

            if (imagem != null &&
                imagem.Length > 0)
            {
                var arquivo =
                    new StreamContent(
                        imagem.OpenReadStream());

                if (!string.IsNullOrWhiteSpace(
                    imagem.ContentType))
                {
                    arquivo.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            imagem.ContentType);
                }

                form.Add(
                    arquivo,
                    "Imagem",
                    imagem.FileName);
            }

            if (video != null &&
                video.Length > 0)
            {
                var arquivo =
                    new StreamContent(
                        video.OpenReadStream());

                if (!string.IsNullOrWhiteSpace(
                    video.ContentType))
                {
                    arquivo.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            video.ContentType);
                }

                form.Add(
                    arquivo,
                    "Video",
                    video.FileName);
            }

            using var requisicao =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/produtos/upload");

            requisicao.Headers.Add(
                "X-Internal-Key",
                _apiKey);

            requisicao.Content = form;

            using var resposta =
                await _http.SendAsync(requisicao);

            if (!resposta.IsSuccessStatusCode)
            {
                return null;
            }

            var stream =
                await resposta.Content.ReadAsStreamAsync();

            return await JsonSerializer
                .DeserializeAsync<Produto>(
                    stream,
                    _json);
        }

        // =========================================================
        // PRODUTOS DISPONÍVEIS - SITE
        // =========================================================

        public async Task<List<Produto>>
            ListarDisponiveisAsync(
                string? categoria = null)
        {
            var url = "api/produtos";

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                url +=
                    $"?categoria={Uri.EscapeDataString(categoria)}";
            }

            var resposta =
                await _http.GetAsync(url);

            if (!resposta.IsSuccessStatusCode)
            {
                return new List<Produto>();
            }

            var stream =
                await resposta.Content.ReadAsStreamAsync();

            return await JsonSerializer
                .DeserializeAsync<List<Produto>>(
                    stream,
                    _json)
                ?? new List<Produto>();
        }
    }
}