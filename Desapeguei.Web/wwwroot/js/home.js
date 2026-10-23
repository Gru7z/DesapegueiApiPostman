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
    const inputPesquisa = document.querySelector(".barra-pesquisa input");

    let todosProdutos = [...produtosPadrao];
    let produtosExibidos = [];

    function mostrarProdutos(lista) {

        produtosExibidos = lista;
        produtosContainer.innerHTML = "";


        if (lista.length === 0) {
            produtosContainer.innerHTML = `<p class="text-center text-white">Nenhum produto encontrado.</p>`;
            return;
        }

        lista.forEach((produto, index) => {

            produtosContainer.innerHTML += `
                <div class="col-6 col-md-4 col-lg-3">

                   <div class="card card-produto h-100 shadow"
                    style="cursor: pointer;"
                    data-produto-index="${index}">

                        <img src="${produto.imagem}" class="card-img-top">

                        <div class="card-body text-center">

                            <h5>${produto.nome}</h5>

                            <p>R$ ${Number(produto.preco).toFixed(2).replace(".", ",")}</p>

                            
                            <button type="button" class="btn btn-success btn-detalhes">
                                Ver detalhes
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
   
    if (inputPesquisa) {
        inputPesquisa.addEventListener("keyup", aplicarFiltros);
    }

   
    // Abre os detalhes quando clicar em um produto
    produtosContainer.addEventListener("click", function (evento) {

        const card = evento.target.closest(".card-produto");

        if (!card) return;

        const index = Number(card.dataset.produtoIndex);
        const produto = produtosExibidos[index];

        if (!produto) return;

        const modalAntigo = document.getElementById("modalDetalhesProduto");

        if (modalAntigo) {
            modalAntigo.remove();
        }

        const modal = document.createElement("div");
        modal.id = "modalDetalhesProduto";

        modal.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.65);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
            padding: 20px;
        `;

        modal.innerHTML = `
            <div style="
                background: #fff8f1;
                color: #54351f;
                padding: 25px;
                border-radius: 15px;
                max-width: 750px;
                width: 100%;
                position: relative;
                display: flex;
                flex-wrap: wrap;
                gap: 20px;
                max-height: 90vh;
                overflow-y: auto;
            ">
                <button type="button" id="fecharDetalhes"
                    style="position:absolute; right:12px; top:8px;
                    border:0; background:none; font-size:26px; cursor:pointer;">
                    &times;
                </button>

                <img src="${produto.imagem}" alt="${produto.nome}"
                    style="width:100%; max-width:280px; object-fit:contain;">

                <div style="flex:1; min-width:200px; padding-top:20px;">
                    <h2>${produto.nome}</h2>

                    <h3 style="color:#a85f2d;">
                        R$ ${Number(produto.preco).toFixed(2).replace(".", ",")}
                    </h3>

                    <p>${produto.descricao || "Sem descrição disponível."}</p>

                    <p>⭐⭐⭐⭐⭐ 5,0 (avaliação demonstrativa)</p>

                    
                   <button type="button" id="comprarAgora" class="btn btn-success">
                    Comprar agora
                   </button>

<button type="button" id="adicionarAoCarrinho"
    class="btn btn-outline-success">
    Adicionar ao carrinho
</button>

                </div>
            </div>
        `;

        document.body.appendChild(modal);

        document.getElementById("fecharDetalhes").addEventListener("click", function () {
            modal.remove();
        });

        modal.addEventListener("click", function (eventoModal) {
            if (eventoModal.target === modal) {
                modal.remove();
            }
        });


        document.getElementById("comprarAgora").addEventListener("click", function () {
            window.location.href = "/Pagamento/Index";
        });

        document.getElementById("adicionarAoCarrinho").addEventListener("click", function () {

            let carrinho = JSON.parse(localStorage.getItem("carrinhoDesapeguei")) || [];

            carrinho.push({
                nome: produto.nome,
                preco: produto.preco,
                imagem: produto.imagem,
                descricao: produto.descricao,
                quantidade: 1
            });

            localStorage.setItem("carrinhoDesapeguei", JSON.stringify(carrinho));

            alert("Produto adicionado ao carrinho!");
        });


    });


    // ORIGINAL: continua carregando os produtos da API
    carregarProdutosDaApi();

});

function comprarProduto(index) {
    window.location.href = "/Pagamento/Index";
}
