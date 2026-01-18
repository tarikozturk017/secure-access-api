using SecureAccess.Api.Domain;

namespace SecureAccess.Api.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _byEmail =
        new(StringComparer.OrdinalIgnoreCase);

    public User? GetByEmail(string email)
        => _byEmail.TryGetValue(email, out var user) ? user : null;

    public void Add(User user)
        => _byEmail[user.Email] = user;
}
