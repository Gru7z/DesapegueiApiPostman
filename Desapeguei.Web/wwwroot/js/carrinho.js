function removerProduto(botao) {

    const confirmar = confirm(
        "Tem certeza que deseja remover este produto do carrinho?"
    );

    if (!confirmar) {
        return;
    }

    const produto = botao.closest(".produto-carrinho");

    produto.remove();

    atualizarResumo();
}


function atualizarResumo() {

    const produtos = document.querySelectorAll(".produto-carrinho");

    let total = 0;

    produtos.forEach(function (produto) {

        const preco = Number(produto.dataset.preco);

        total = total + preco;
    });

    const valorFormatado = total.toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL"
    });

    document.getElementById("subtotal").textContent = valorFormatado;

    document.getElementById("total").textContent = valorFormatado;
}