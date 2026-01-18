using Microsoft.AspNetCore.Identity;
using SecureAccess.Api.Domain;
using SecureAccess.Api.Repositories;

namespace SecureAccess.Api.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(IUserRepository users)
    {
        _users = users;
    }

    public User Register(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Email and password are required.");

        if (_users.GetByEmail(email) is not null)
            throw new InvalidOperationException("User already exists.");

        var temp = new User(Guid.NewGuid(), email.Trim(), "");
        var hash = _hasher.HashPassword(temp, password);

        var user = temp with { PasswordHash = hash };
        _users.Add(user);

        return user;
    }

    public User Login(string email, string password)
    {
        var user = _users.GetByEmail(email)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials.");

        return user;
    }
}
