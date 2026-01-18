using SecureAccess.Api.Domain;

namespace SecureAccess.Api.Services; 

public interface IAuthService
{
    User Register(string email, string password);
    User Login(string email, string password);
}
