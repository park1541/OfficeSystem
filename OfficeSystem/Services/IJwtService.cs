using OfficeSystem.Models;

namespace OfficeSystem.Services
{
    public interface IJwtService
    {
        string GenerateToken (User user);
    }
}
