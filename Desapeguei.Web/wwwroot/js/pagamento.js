document.addEventListener("DOMContentLoaded", function () {

    // ---------- SELEÇÃO DA FORMA DE PAGAMENTO ----------
    const opcoes = document.querySelectorAll(".opcao-pagamento");

    opcoes.forEach(opcao => {
        opcao.addEventListener("click", function () {
            opcoes.forEach(o => o.classList.remove("selecionada"));
            opcao.classList.add("selecionada");
            opcao.querySelector("input[type=radio]").checked = true;
        });
    });

    if (opcoes.length > 0) {
        opcoes[0].classList.add("selecionada");
    }

    // ---------- CONFIRMAR FORMA DE PAGAMENTO ----------
    const btnConfirmar = document.getElementById("btnConfirmarPagamento");

    if (btnConfirmar) {
        btnConfirmar.addEventListener("click", function () {
            const selecionado = document.querySelector("input[name=formaPagamento]:checked").value;

            if (selecionado === "cartao") {
                window.location.href = "/Pagamento/CartaoCredito";
            } else if (selecionado === "pix") {
                alert("PIX gerado! Escaneie o QR Code para concluir o pagamento (simulação).");
            } else {
                alert("Boleto gerado! Vencimento em 3 dias úteis (simulação).");
            }
        });
    }

    // ---------- CONFIRMAR PAGAMENTO NO CARTÃO ----------
    const btnConfirmarCartao = document.getElementById("btnConfirmarCartao");

    if (btnConfirmarCartao) {
        btnConfirmarCartao.addEventListener("click", function () {
            const numero = document.getElementById("numeroCartao").value.trim();
            const validade = document.getElementById("validadeCartao").value.trim();
            const cvv = document.getElementById("cvvCartao").value.trim();
            const nome = document.getElementById("nomeCartao").value.trim();

            if (!numero || !validade || !cvv || !nome) {
                alert("Preencha todos os campos do cartão.");
                return;
            }

            alert("Pagamento confirmado com sucesso! (simulação)");
            window.location.href = "/Home/Index";
        });
    }

    // ---------- MÁSCARAS SIMPLES ----------
    const numeroCartao = document.getElementById("numeroCartao");
    if (numeroCartao) {
        numeroCartao.addEventListener("input", function () {
            numeroCartao.value = numeroCartao.value
                .replace(/\D/g, "")
                .replace(/(.{4})/g, "$1 ")
                .trim();
        });
    }

    const validadeCartao = document.getElementById("validadeCartao");
    if (validadeCartao) {
        validadeCartao.addEventListener("input", function () {
            validadeCartao.value = validadeCartao.value
                .replace(/\D/g, "")
                .replace(/(\d{2})(\d)/, "$1/$2");
        });
    }

});
