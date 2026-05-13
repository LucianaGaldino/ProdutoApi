using MySql.Data.MySqlClient;
using ProdutoAPI.Database;
using ProdutoAPI.Models;

namespace ProdutoAPI.Repositories;

public class ProdutoRepository
{
    private readonly DatabaseConnection _db;

    public ProdutoRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public async Task<List<Produto>> ListarTodosAsync()
    {
        const string sql = "SELECT id, nome, preco, quantidade FROM produtos ORDER BY nome";
        var produtos = new List<Produto>();

        await using var connection = _db.CreateConnection();
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        await using var reader = (MySqlDataReader)await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            produtos.Add(MapearProduto(reader));
        }

        return produtos;
    }

    public async Task<Produto?> BuscarPorIdAsync(int id)
    {
        const string sql = "SELECT id, nome, preco, quantidade FROM produtos WHERE id = @Id";

        await using var connection = _db.CreateConnection();
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await using var reader = (MySqlDataReader)await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return MapearProduto(reader);

        return null;
    }

    public async Task<Produto> CadastrarAsync(Produto produto)
    {
        const string sql = @"
            INSERT INTO produtos (nome, preco, quantidade)
            VALUES (@Nome, @Preco, @Quantidade);
            SELECT LAST_INSERT_ID();";

        await using var connection = _db.CreateConnection();
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Nome", produto.Nome);
        command.Parameters.AddWithValue("@Preco", produto.Preco);
        command.Parameters.AddWithValue("@Quantidade", produto.Quantidade);

        var newId = Convert.ToInt32(await command.ExecuteScalarAsync());
        produto.Id = newId;
        return produto;
    }

    public async Task<bool> AtualizarAsync(Produto produto)
    {
        const string sql = @"
            UPDATE produtos
            SET nome = @Nome, preco = @Preco, quantidade = @Quantidade
            WHERE id = @Id";

        await using var connection = _db.CreateConnection();
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", produto.Id);
        command.Parameters.AddWithValue("@Nome", produto.Nome);
        command.Parameters.AddWithValue("@Preco", produto.Preco);
        command.Parameters.AddWithValue("@Quantidade", produto.Quantidade);

        var linhasAfetadas = await command.ExecuteNonQueryAsync();
        return linhasAfetadas > 0;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        const string sql = "DELETE FROM produtos WHERE id = @Id";

        await using var connection = _db.CreateConnection();
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var linhasAfetadas = await command.ExecuteNonQueryAsync();
        return linhasAfetadas > 0;
    }

    public async Task<bool> ExisteAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM produtos WHERE id = @Id";

        await using var connection = _db.CreateConnection();
        await connection.OpenAsync();
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        return count > 0;
    }

    private static Produto MapearProduto(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        Nome = reader.GetString("nome"),
        Preco = reader.GetDecimal("preco"),
        Quantidade = reader.GetInt32("quantidade")
    };
}
