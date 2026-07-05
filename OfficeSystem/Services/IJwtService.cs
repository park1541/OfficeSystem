using OfficeSystem.Models.Users;

namespace OfficeSystem.Services
{
    public interface IJwtService
    {
        string GenerateToken (User user);
    }
}
