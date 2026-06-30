using OfficeSystem.DTOs;
using OfficeSystem.Models;

namespace OfficeSystem.Services
{
    public interface IUserService
    {
        Task<User> Register(RegisterRequest request);
        Task<User?> Login(LoginRequest login);
    }
}
