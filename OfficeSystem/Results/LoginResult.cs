using OfficeSystem.Models.Users;

namespace OfficeSystem.Results
{
    public class LoginResult
    {
        public required User User { get; set; }
        public required string Token { get; set; } 
    }
}
