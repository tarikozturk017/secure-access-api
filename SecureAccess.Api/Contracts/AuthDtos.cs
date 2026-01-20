namespace SecureAccess.Api.Contracts;

public sealed record RegisterRequest(string Email, string Password);
public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(Guid UserId, string Email, string AccessToken);


public sealed record AuthResponse(Guid UserId, string Email);
