document.addEventListener("DOMContentLoaded", function () {

    // =====================================================
    // ELEMENTOS
    // =====================================================

    const inputImagem = document.getElementById("inputImagem");
    const inputVideo = document.getElementById("inputVideo");

    const iconeImagem = document.getElementById("iconeImagem");
    const textoImagem = document.getElementById("textoImagem");

    const iconeVideo = document.getElementById("iconeVideo");
    const textoVideo = document.getElementById("textoVideo");
    const previewVideo = document.getElementById("previewVideo");

    const precoProduto = document.getElementById("precoProduto");

    const form = document.getElementById("formVenderProduto");
    const mensagemVender = document.getElementById("mensagemVender");


    // =====================================================
    // PREVIEW DA IMAGEM
    // =====================================================

    inputImagem.addEventListener("change", function () {

        const arquivo = inputImagem.files[0];

        if (!arquivo) return;

        if (!arquivo.type.startsWith("image/")) {

            alert("Selecione uma imagem válida.");

            inputImagem.value = "";

            return;
        }

        const leitor = new FileReader();

        leitor.onload = function (e) {

            const box = inputImagem.previousElementSibling;

            box.innerHTML = `
                <img src="${e.target.result}" 
                     class="preview-upload">
            `;
        };

        leitor.readAsDataURL(arquivo);
    });


    // =====================================================
    // PREVIEW DO VÍDEO
    // =====================================================

    inputVideo.addEventListener("change", function () {

        const arquivo = inputVideo.files[0];

        if (!arquivo) return;

        // Verifica se é vídeo
        if (!arquivo.type.startsWith("video/")) {

            alert("Selecione um arquivo de vídeo.");

            inputVideo.value = "";

            return;
        }

        // Cria URL temporária
        const url = URL.createObjectURL(arquivo);

        // Coloca o vídeo no elemento <video>
        previewVideo.src = url;

        // Mostra o vídeo
        previewVideo.style.display = "block";

        // Altera o texto
        // Esconde o texto do nome do arquivo
        textoVideo.style.display = "none";

        // Esconde o ícone
        iconeVideo.style.display = "none";
    });


    // =====================================================
    // MÁSCARA DE PREÇO
    // =====================================================

    precoProduto.addEventListener("input", function () {

        let valor = precoProduto.value.replace(/\D/g, "");

        valor = (Number(valor) / 100).toFixed(2);

        valor = valor.replace(".", ",");

        precoProduto.value = "R$ " + valor;
    });


    // =====================================================
    // ENVIO DO FORMULÁRIO
    // =====================================================

    form.addEventListener("submit", async function (event) {

        event.preventDefault();

        const categoriaSelecionada =
            document.querySelector("input[name=Categoria]:checked");


        // Verifica categoria

        if (!categoriaSelecionada) {

            mostrarMensagem(
                "Selecione uma categoria antes de publicar.",
                "danger"
            );

            return;
        }


        // =================================================
        // CONVERTE PREÇO
        // =================================================

        const precoNumerico = precoProduto.value
            .replace("R$", "")
            .replace(/\./g, "")
            .trim();


        // =================================================
        // CRIA FORMDATA
        // =================================================

        const dadosFormulario = new FormData();


        // Texto

        dadosFormulario.append(
            "Titulo",
            document.getElementById("tituloProduto").value
        );

        dadosFormulario.append(
            "Descricao",
            document.getElementById("descricaoProduto").value
        );

        dadosFormulario.append(
            "Categoria",
            categoriaSelecionada.value
        );

        dadosFormulario.append(
            "Preco",
            precoNumerico
        );


        // =================================================
        // IMAGEM
        // =================================================

        if (inputImagem.files.length > 0) {

            dadosFormulario.append(
                "Imagem",
                inputImagem.files[0]
            );
        }


        // =================================================
        // VÍDEO
        // =================================================

        if (inputVideo.files.length > 0) {

            dadosFormulario.append(
                "Video",
                inputVideo.files[0]
            );
        }


        // =================================================
        // BOTÃO
        // =================================================

        const botaoPublicar =
            form.querySelector(".btn-publicar");

        botaoPublicar.disabled = true;

        botaoPublicar.textContent = "Publicando...";


        // =================================================
        // ENVIA PARA API
        // =================================================

        try {

            const resposta = await fetch(
                `${API_BASE}/api/produtos/upload`,
                {
                    method: "POST",
                    body: dadosFormulario
                }
            );


            // =============================================
            // SUCESSO
            // =============================================

            if (resposta.ok) {

                mostrarMensagem(
                    "Produto publicado com sucesso!",
                    "success"
                );

                form.reset();

                // Limpa preview do vídeo
                previewVideo.src = "";
                previewVideo.style.display = "none";

                // Mostra novamente o ícone
                if (iconeVideo) {
                    iconeVideo.style.display = "block";
                }

                if (textoVideo) {
                    textoVideo.textContent = "Adicionar vídeo";
                }


                setTimeout(function () {

                    window.location.href = "/Home/Index";

                }, 1200);

            }


            // =============================================
            // NÃO AUTORIZADO
            // =============================================

            else if (resposta.status === 401) {

                mostrarMensagem(
                    "Você precisa estar logado para publicar um produto.",
                    "danger"
                );

            }


            // =============================================
            // OUTRO ERRO
            // =============================================

            else {

                let erro;

                try {
                    erro = await resposta.json();
                }
                catch {
                    erro = {};
                }

                mostrarMensagem(
                    erro.mensagem ||
                    "Não foi possível publicar o produto. Verifique os campos.",
                    "danger"
                );
            }

        }


        // =================================================
        // ERRO DE CONEXÃO
        // =================================================

        catch (erro) {

            console.error(erro);

            mostrarMensagem(
                "Erro de conexão ao publicar o produto.",
                "danger"
            );
        }


        // =================================================
        // DEVOLVE BOTÃO
        // =================================================

        finally {

            botaoPublicar.disabled = false;

            botaoPublicar.textContent = "PUBLICAR";
        }

    });


    // =====================================================
    // MENSAGEM
    // =====================================================

    function mostrarMensagem(texto, tipo) {

        mensagemVender.innerHTML = `
            <div class="alert alert-${tipo}">
                ${texto}
            </div>
        `;
    }

});