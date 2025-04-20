using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Core.Interfaces;

public interface IUserService
{
    Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User?> AddAndUpdateUser(User userObj);
}