namespace SecureAccess.Api.Repositories;

public interface IAuditLogRepository
{
    void Add(string eventType, string payload, DateTime occurredAtUtc);
}
