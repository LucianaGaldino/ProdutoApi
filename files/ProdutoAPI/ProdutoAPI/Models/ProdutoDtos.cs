namespace ProdutoAPI.Models;

public record ProdutoRequest(
    string Nome,
    decimal Preco,
    int Quantidade
);

public record ProdutoResponse(
    int Id,
    string Nome,
    decimal Preco,
    int Quantidade
);

public record EstoqueResponse(
    int TotalProdutos,
    decimal ValorTotal
);
