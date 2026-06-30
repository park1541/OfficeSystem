namespace OfficeSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string LoginId { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string EmployeeNumber {  get; set; } = string.Empty;

        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
