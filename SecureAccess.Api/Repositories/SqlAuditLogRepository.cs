using Dapper;
using Microsoft.Data.SqlClient;

namespace SecureAccess.Api.Repositories;

public sealed class  SqlAuditLogRepository : IAuditLogRepository
{
    private readonly string _connStr;
    public SqlAuditLogRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string: Default");
    }

    public void Add(string eventType, string payload, DateTime occuredAtUtc)
    {
        using var conn = new SqlConnection(_connStr);
        
        const string sql = """
            INSERT INTO dbo.AutitLogs (Id, EventType, Payload, OccuredAtUtc)
            VALUES (@Id, @EventType, @Payload, @OccuredAtUtc)
            """; 

        conn.Execute(sql, new
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            Payload = payload,
            OccuredAtUtc = occuredAtUtc
        });
    }
}