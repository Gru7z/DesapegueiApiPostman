document.addEventListener("DOMContentLoaded", function () {

    const produtosPadrao = [
        {
            nome: "Pulseira",
            preco: 18.90,
            categoria: "Acessórios",
            imagem: "/img/produto1.jpg",
            descricao: "Pulseira artesanal feita com miçangas coloridas."
        },
        {
            nome: "Boneca",
            preco: 25.00,
            categoria: "Brinquedos",
            imagem: "/img/produto2.jpg",
            descricao: "Boneca divertida, perfeita para crianças."
        },
        {
            nome: "Tênis da Adidas",
            preco: 120.00,
            categoria: "Calçados",
            imagem: "/img/produto3.jpg",
            descricao: "Tênis esportivo da Adidas, confortável e estiloso."
        },
        {
            nome: "Camiseta Oversized",
            preco: 45.00,
            categoria: "Roupas",
            imagem: "/img/produto4.jpg",
            descricao: "Camiseta oversized, ideal para um look casual e moderno."
        },
        {
            nome: "Mesa Gamer",
            preco: 350.00,
            categoria: "Móveis",
            imagem: "/img/produto5.jpg",
            descricao: "Mesa gamer com design ergonômico e espaço para acessórios."
        },
        {
            nome: "Calça cargo",
            preco: 350.00,
            categoria: "Roupas",
            imagem: "/img/produto6.jpg",
            descricao: "Calça cargo, perfeita para um estilo urbano e confortável."
        },
    ];

    const produtosContainer = document.getElementById("produtosContainer");

    // Página não é o marketplace (ex: Vender, Pagamento) -> não há nada a listar aqui.
    if (!produtosContainer) return;

    const categoriaFiltro = (produtosContainer.dataset.categoria || "").trim();

    let todosProdutos = [...produtosPadrao];

    function mostrarProdutos(lista) {

        produtosContainer.innerHTML = "";

        if (lista.length === 0) {
            produtosContainer.innerHTML = `<p class="text-center text-white">Nenhum produto encontrado.</p>`;
            return;
        }

        lista.forEach((produto, index) => {

            produtosContainer.innerHTML += `
                <div class="col-6 col-md-4 col-lg-3">

                    <div class="card card-produto h-100 shadow">

                        <img src="${produto.imagem}" class="card-img-top">

                        <div class="card-body text-center">

                            <h5>${produto.nome}</h5>

                            <p>R$ ${Number(produto.preco).toFixed(2).replace(".", ",")}</p>

                            <button onclick="comprarProduto(${index})" class="btn btn-success">
                                Comprar
                            </button>

                        </div>

                    </div>

                </div>
            `;
        });
    }

    function aplicarFiltros() {
        const texto = inputPesquisa ? inputPesquisa.value.toLowerCase() : "";

        let filtrados = todosProdutos;

        if (categoriaFiltro) {
            filtrados = filtrados.filter(p =>
                p.categoria.toLowerCase() === categoriaFiltro.toLowerCase()
            );
        }

        if (texto) {
            filtrados = filtrados.filter(p =>
                p.nome.toLowerCase().includes(texto) ||
                p.categoria.toLowerCase().includes(texto)
            );
        }

        mostrarProdutos(filtrados);
    }

    // ---------- BUSCA OS PRODUTOS REAIS CADASTRADOS PELOS USUÁRIOS (API) ----------
    async function carregarProdutosDaApi() {
        try {
            const resposta = await fetch(`${API_BASE}/api/produtos`);
            if (!resposta.ok) return;

            const produtosApi = await resposta.json();

            const produtosConvertidos = produtosApi.map(p => ({
                nome: p.titulo,
                preco: p.preco,
                categoria: p.categoria,
                imagem: p.imagemUrl ? `${API_BASE}${p.imagemUrl}` : "/img/produto1.jpg",
                descricao: p.descricao
            }));

            todosProdutos = [...produtosConvertidos, ...produtosPadrao];
            aplicarFiltros();
        } catch (erro) {
            // Sem conexão com a API: mantém apenas os produtos de exemplo.
            console.warn("Não foi possível carregar os produtos cadastrados pelos usuários.", erro);
        }
    }

    mostrarProdutos(todosProdutos.filter(p =>
        !categoriaFiltro || p.categoria.toLowerCase() === categoriaFiltro.toLowerCase()
    ));

    const inputPesquisa = document.querySelector(".barra-pesquisa input");

    if (inputPesquisa) {
        inputPesquisa.addEventListener("keyup", aplicarFiltros);
    }

    carregarProdutosDaApi();

});

function comprarProduto(index) {
    window.location.href = "/Pagamento/Index";
}
