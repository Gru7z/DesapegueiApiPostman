
document.addEventListener("DOMContentLoaded", function () {

    mostrarProdutos();
    atualizarResumo();

});


function mostrarProdutos() {

    const listaCarrinho = document.getElementById("listaCarrinho");

    const carrinho = JSON.parse(
        localStorage.getItem("carrinhoDesapeguei")
    ) || [];

    listaCarrinho.innerHTML = "";

    if (carrinho.length === 0) {
        listaCarrinho.innerHTML = "<p>Seu carrinho está vazio.</p>";
        return;
    }

    carrinho.forEach(function (produto, index) {

        listaCarrinho.innerHTML += `
            <div class="produto-carrinho"
                 data-preco="${produto.preco}">

                <img src="${produto.imagem}"
                     class="imagem-produto"
                     alt="${produto.nome}">

                <div class="info-produto">
                    <h4>${produto.nome}</h4>

                    <p class="preco-produto">
                        ${Number(produto.preco).toLocaleString("pt-BR", {
            style: "currency",
            currency: "BRL"
        })}
                    </p>
                </div>

                <button type="button"
                        class="btn-remover"
                        onclick="removerProduto(this)"
                        data-index="${index}"
                        aria-label="Remover produto">
                    🗑
                </button>

            </div>
        `;
    });
}


function removerProduto(botao) {

    const confirmar = confirm(
        "Tem certeza que deseja remover este produto do carrinho?"
    );

    if (!confirmar) {
        return;
    }

    const index = Number(botao.dataset.index);

    let carrinho = JSON.parse(
        localStorage.getItem("carrinhoDesapeguei")
    ) || [];

    carrinho.splice(index, 1);

    localStorage.setItem(
        "carrinhoDesapeguei",
        JSON.stringify(carrinho)
    );

    mostrarProdutos();
    atualizarResumo();
}


function atualizarResumo() {

    const carrinho = JSON.parse(
        localStorage.getItem("carrinhoDesapeguei")
    ) || [];

    let total = 0;

    carrinho.forEach(function (produto) {
        total += Number(produto.preco) * Number(produto.quantidade || 1);
    });

    const valorFormatado = total.toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL"
    });

    document.getElementById("subtotal").textContent = valorFormatado;
    document.getElementById("total").textContent = valorFormatado;
}
