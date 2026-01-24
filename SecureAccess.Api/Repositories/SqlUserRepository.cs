using Dapper;
using Microsoft.Data.SqlClient;
using SecureAccess.Api.Domain;

namespace SecureAccess.Api.Repositories;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly string _connStr;
    public SqlUserRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string: Default");
    }

    public User? GetByEmail(string email)
    {
        using var conn = new SqlConnection(_connStr);
        const string sql = """
            SELECT TOP 1 Id, Email, PasswordHash
            FROM dbo.Users
            WHERE Email = @Email
            """;

        return conn.QuerySingleOrDefault<User>(sql, new { Email = email });
    }

    public void Add(User user)
    {
        using var conn = new SqlConnection(_connStr);
        const string sql = """
            INSERT INTO dbo.Users (Id, Email, PasswordHash)
            VALUES (@Id, @Email, @PasswordHash)
            """;

        conn.Execute(sql, user);
    }
}