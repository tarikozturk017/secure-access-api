namespace SecureAccess.Api.Contracts;

public sealed record UserLoggedInEvent(Guid UserId, string Email, DateTime OccurredAtUtc);
