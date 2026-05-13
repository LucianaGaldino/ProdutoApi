using ProdutoAPI.Enums;
using ProdutoAPI.Exceptions;
using ProdutoAPI.Models;
using ProdutoAPI.Repositories;

namespace ProdutoAPI.Services;

public class ProdutoService
{
    private readonly ProdutoRepository _repository;

    private const decimal PrecoMinimo = 0.01m;
    private const int QuantidadeMinima = 0;
    private const int NomeTamanhoMinimo = 2;
    private const int NomeTamanhoMaximo = 100;

    public ProdutoService(ProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProdutoResponse>> ListarTodosAsync(string? ordenarPor = null)
    {
        var produtos = await _repository.ListarTodosAsync();

        var ordenados = ordenarPor?.ToLower() switch
        {
            "preco" => produtos.OrderBy(p => p.Preco).ToList(),
            "quantidade" => produtos.OrderBy(p => p.Quantidade).ToList(),
            "nome" => produtos.OrderBy(p => p.Nome).ToList(),
            _ => produtos.OrderBy(p => p.Id).ToList()
        };

        return ordenados.Select(MapearParaResponse).ToList();
    }

    public async Task<ProdutoResponse> BuscarPorIdAsync(int id)
    {
        ValidarId(id);
        var produto = await _repository.BuscarPorIdAsync(id)
            ?? throw new AppException($"Produto com Id {id} não encontrado.", StatusCode.NotFound);

        return MapearParaResponse(produto);
    }

    public async Task<ProdutoResponse> CadastrarAsync(ProdutoRequest request)
    {
        ValidarRequest(request);

        var produto = new Produto
        {
            Nome = request.Nome.Trim(),
            Preco = request.Preco,
            Quantidade = request.Quantidade
        };

        var criado = await _repository.CadastrarAsync(produto);
        return MapearParaResponse(criado);
    }

    public async Task<ProdutoResponse> AtualizarAsync(int id, ProdutoRequest request)
    {
        ValidarId(id);
        ValidarRequest(request);

        var existe = await _repository.ExisteAsync(id);
        if (!existe)
            throw new AppException($"Produto com Id {id} não encontrado.", StatusCode.NotFound);

        var produto = new Produto
        {
            Id = id,
            Nome = request.Nome.Trim(),
            Preco = request.Preco,
            Quantidade = request.Quantidade
        };

        await _repository.AtualizarAsync(produto);
        return MapearParaResponse(produto);
    }

    public async Task RemoverAsync(int id)
    {
        ValidarId(id);

        var removido = await _repository.RemoverAsync(id);
        if (!removido)
            throw new AppException($"Produto com Id {id} não encontrado.", StatusCode.NotFound);
    }

    public async Task<EstoqueResponse> CalcularEstoqueAsync()
    {
        var produtos = await _repository.ListarTodosAsync();

        var totalProdutos = produtos.Count;
        var valorTotal = produtos.Sum(p => p.Preco * p.Quantidade);

        return new EstoqueResponse(totalProdutos, valorTotal);
    }

    private void ValidarRequest(ProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new AppException("O nome do produto é obrigatório.", StatusCode.BadRequest);

        if (request.Nome.Trim().Length < NomeTamanhoMinimo)
            throw new AppException($"O nome deve ter no mínimo {NomeTamanhoMinimo} caracteres.", StatusCode.BadRequest);

        if (request.Nome.Trim().Length > NomeTamanhoMaximo)
            throw new AppException($"O nome deve ter no máximo {NomeTamanhoMaximo} caracteres.", StatusCode.BadRequest);

        if (request.Preco < PrecoMinimo)
            throw new AppException($"O preço deve ser maior que {PrecoMinimo:C}.", StatusCode.BadRequest);

        if (request.Quantidade < QuantidadeMinima)
            throw new AppException("A quantidade não pode ser negativa.", StatusCode.BadRequest);
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new AppException("O Id deve ser um número positivo.", StatusCode.BadRequest);
    }

    private static ProdutoResponse MapearParaResponse(Produto produto) =>
        new(produto.Id, produto.Nome, produto.Preco, produto.Quantidade);
}
