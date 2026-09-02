using Dapper;
using SimpleAuthAPI.Data;
using SimpleAuthAPI.Model;

namespace SimpleAuthAPI.Repository;

public interface IAuthRepository
{
    public Task<int> Register(RegisterUserRequest request);
    public Task<bool> IsUserExistsByEmail(string email);
    public Task<LoginUserResponse?> GetUserByEmail(string email);
    public Task<List<string>> GetAll();
}

public class AuthRepository(ConnectionFactory connection) : IAuthRepository
{
    public async Task<int> Register(RegisterUserRequest request)
    {
        string sql = "INSERT INTO users (email, password_hash) VALUES (@Email, @Password) RETURNING user_id";
        using var conn = connection.Create();

        return await conn.QuerySingleAsync<int>(sql, new { Email = request.Email, Password = request.Password });
    }
    public async Task<bool> IsUserExistsByEmail(string email)
    {
        string sql = "SELECT EXISTS (SELECT 1 FROM users WHERE email = @Email)";
        using var conn = connection.Create();

        var result = await conn.QuerySingleAsync<bool>(sql, new { Email = email } );
        return result;
    }

    public async Task<LoginUserResponse?> GetUserByEmail(string email)
    {
        string sql = "SELECT user_id as UserId, password_hash as PasswordHashed, email FROM users WHERE email = @Email";
        using var conn = connection.Create();

        var result = await conn.QuerySingleOrDefaultAsync<LoginUserResponse>(sql, new { Email = email });
        return result;
    }

    public async Task<List<string>> GetAll()
    {
        string sql = "SELECT email FROM users";
        using var conn = connection.Create();

        var result = await conn.QueryAsync<string>(sql);
        return result.ToList();
    }
}
