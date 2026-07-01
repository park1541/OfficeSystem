using OfficeSystem.Models;

namespace OfficeSystem.DTOs
{
    public class LoginResult
    {
        public required User User { get; set; }
        public required string Token { get; set; } 
    }
}
