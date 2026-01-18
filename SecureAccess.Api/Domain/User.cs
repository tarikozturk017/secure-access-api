namespace SecureAccess.Api.Domain;

public sealed record User(Guid Id, string Email, string PasswordHash);
