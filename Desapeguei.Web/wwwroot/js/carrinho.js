
.carrinho-container {
    max-width: 1000px;
    margin: 40px auto;
    padding: 20px;
}

.titulo-carrinho {
    color: #6f472f;
    font-size: 2rem;
    margin-bottom: 25px;
    font-weight: bold;
}

/* PRODUTOS */

.produto-carrinho {
    display: flex;
    align-items: center;
    gap: 20px;

    background: #a85f2d;
    border-radius: 15px;

    padding: 20px;
    margin-bottom: 15px;

    color: white;
}

.imagem-produto {
    width: 120px;
    height: 120px;

    object-fit: cover;

    border-radius: 12px;
    background: white;
}

.info-produto {
    flex: 1;
}

.info-produto h4 {
    margin: 0 0 5px;
    font-size: 1.3rem;
}

.categoria-produto {
    margin: 0;
    color: #f3d4bc;
}

.preco-produto {
    margin: 8px 0;
    font-size: 1.2rem;
    font-weight: bold;
}

/* QUANTIDADE */

.quantidade {
    display: flex;
    align-items: center;
    gap: 12px;
}

.btn-quantidade {
    width: 32px;
    height: 32px;

    border: none;
    border-radius: 8px;

    background: #d3a07b;
    color: white;

    font-size: 20px;
    font-weight: bold;

    cursor: pointer;
}

.quantidade-valor {
    font-weight: bold;
}

/* REMOVER */

.btn-remover {
    border: none;
    background: transparent;

    color: white;
    font-size: 22px;

    cursor: pointer;
}

.btn-remover:hover {
    color: #ffe16b;
}

/* RESUMO */

.resumo-carrinho {
    background: #a85f2d;

    border-radius: 15px;

    padding: 25px;

    margin-top: 25px;

    color: white;
}

.resumo-carrinho h3 {
    margin-bottom: 20px;
}

.linha-resumo,
.linha-total {
    display: flex;
    justify-content: space-between;

    margin-bottom: 12px;
}

.linha-total {
    font-size: 1.4rem;
}

/* BOTÃO */

.btn-finalizar {
    width: 100%;

    margin-top: 20px;

    height: 50px;

    border: none;
    border-radius: 12px;

    background: #d3a07b;
    color: white;

    font-size: 1.1rem;
    font-weight: bold;

    cursor: pointer;
}

.btn-finalizar:hover {
    background: #6f472f;
}

/* CELULAR */

@media (max-width: 768px) {

    .carrinho-container {
        margin: 20px auto;
        padding: 15px;
    }

    .produto-carrinho {
        padding: 15px;
        gap: 12px;
    }

    .imagem-produto {
        width: 85px;
        height: 85px;
    }

    .info-produto h4 {
        font-size: 1rem;
    }

    .preco-produto {
        font-size: 1rem;
    }
}

