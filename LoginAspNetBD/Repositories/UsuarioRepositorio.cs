using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;

namespace LoginAspNetBD.Repositories;

public class UserRepository : IUsuarioRepositorio
{
    private readonly string _connectionString;
    private readonly PasswordHasher<UsuarioModel> _passwordHasher;

    public UsuarioRepositorio(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Connection") ?? string.Empty;
        _passwordHasher = new PasswordHasher<UsuarioModel>();
    }

    public async Task<bool> Cadastrar(UsuarioModel usuario, string senhaPura)
    {
        try
        {
            usuario.senhaHash = _passwordHasher.HashPassword(usuario, senhaPura);

            using var conexao = new MySqlConnection(_connectionString);

            await conexao.OpenAsync();

            var query = "INSERT INTO usuarios (Nome, Email, SenhaHash) VALUES (@Nome, @Email, @SenhaHash)";

            using var comando = new MySqlCommand(query, conexao);

            comando.Parameters.AddWithValue("@Nome", usuario.Nome);
            comando.Parameters.AddWithValue("@Email", usuario.Email);
            comando.Parameters.AddWithValue("@SenhaHash", usuario.SenhaHash);

            var linhasAfetadas = await comando.ExecuteNonQueryAsync();

            return linhasAfetadas > 0;
        }
        catch(Exception ex)
        {
            throw new Exception($"Erro no MySql {ex.Message}");
        }

        public async Task<UsuarioModel?> BuscarEmail(string email)
        {
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();
            
            var query = "SELECT Id, Nome, Email, SenhaHash FROM usuarios WHERE Email = @Email";
            
            using var comando = new MySqlCommand(query, conexao);
            
            comando.Parameters.AddWithValue("@Email", email);
            
            using var reader = await comando.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UsuarioModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Nome = reader.GetString("Nome"),
                    Email = reader.GetString("Email"),
                    SenhaHash = reader.GetString("SenhaHash")
                };
            }

            return null;
        }
    }
}