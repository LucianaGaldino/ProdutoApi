using Microsoft.AspNetCore.Mvc;
using ProdutoAPI.Exceptions;
using ProdutoAPI.Models;
using ProdutoAPI.Services;
using ProdutoAPI.Enums;

namespace ProdutoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutoController : ControllerBase
{
    private readonly ProdutoService _service;
    private readonly ILogger<ProdutoController> _logger;

    public ProdutoController(ProdutoService service, ILogger<ProdutoController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>Lista todos os produtos cadastrados.</summary>
    /// <param name="ordenarPor">Campo para ordenação: nome, preco ou quantidade (padrão: id)</param>
    /// <response code="200">Lista de produtos retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProdutoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListarTodos([FromQuery] string? ordenarPor = null)
    {
        try
        {
            var produtos = await _service.ListarTodosAsync(ordenarPor);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return TratarErroInesperado(ex);
        }
    }

    /// <summary>Busca um produto pelo Id.</summary>
    /// <param name="id">Id do produto</param>
    /// <response code="200">Produto encontrado</response>
    /// <response code="400">Id inválido</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            var produto = await _service.BuscarPorIdAsync(id);
            return Ok(produto);
        }
        catch (AppException ex)
        {
            return TratarAppException(ex);
        }
        catch (Exception ex)
        {
            return TratarErroInesperado(ex);
        }
    }

    /// <summary>Cadastra um novo produto.</summary>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] ProdutoRequest request)
    {
        try
        {
            var produto = await _service.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = produto.Id }, produto);
        }
        catch (AppException ex)
        {
            return TratarAppException(ex);
        }
        catch (Exception ex)
        {
            return TratarErroInesperado(ex);
        }
    }

    /// <summary>Atualiza um produto existente.</summary>
    /// <param name="id">Id do produto</param>
    /// <response code="200">Produto atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoRequest request)
    {
        try
        {
            var produto = await _service.AtualizarAsync(id, request);
            return Ok(produto);
        }
        catch (AppException ex)
        {
            return TratarAppException(ex);
        }
        catch (Exception ex)
        {
            return TratarErroInesperado(ex);
        }
    }

    /// <summary>Remove um produto pelo Id.</summary>
    /// <param name="id">Id do produto</param>
    /// <response code="204">Produto removido com sucesso</response>
    /// <response code="400">Id inválido</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
        catch (AppException ex)
        {
            return TratarAppException(ex);
        }
        catch (Exception ex)
        {
            return TratarErroInesperado(ex);
        }
    }

    /// <summary>Retorna o valor total do estoque.</summary>
    /// <response code="200">Valor total calculado com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("estoque")]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalcularEstoque()
    {
        try
        {
            var estoque = await _service.CalcularEstoqueAsync();
            return Ok(estoque);
        }
        catch (Exception ex)
        {
            return TratarErroInesperado(ex);
        }
    }

    private IActionResult TratarAppException(AppException ex)
    {
        _logger.LogWarning("Erro de aplicação: {Message}", ex.Message);
        var statusHttp = (int)ex.StatusCode;
        return base.StatusCode(statusHttp, new ApiErrorResponse(statusHttp, ex.Message));
    }

    private IActionResult TratarErroInesperado(Exception ex)
    {
        _logger.LogError(ex, "Erro inesperado: {Message}", ex.Message);
        return base.StatusCode(
            (int)ProdutoAPI.Enums.StatusCode.InternalError,
            new ApiErrorResponse((int)ProdutoAPI.Enums.StatusCode.InternalError, "Ocorreu um erro interno. Tente novamente mais tarde.")
        );
    }
}

public record ApiErrorResponse(int StatusCode, string Mensagem);
