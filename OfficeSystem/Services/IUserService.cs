using OfficeSystem.DTOs.Auth;
using OfficeSystem.Models.Users;
using OfficeSystem.Results;

namespace OfficeSystem.Services
{
    public interface IUserService
    {
        Task<User?> Register(RegisterRequest request);
        Task<LoginResult?> Login(LoginRequest login);
    }
}
