using SecureAccess.Api.Domain;

namespace SecureAccess.Api.Repositories;


public interface IUserRepository
{
    User? GetByEmail(string email);
    void Add(User user);
}
