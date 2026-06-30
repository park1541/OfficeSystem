using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using OfficeSystem.Data;
using OfficeSystem.DTOs;
using OfficeSystem.Models;

namespace OfficeSystem.Services
{
    public class UserService : IUserService
    {
        private readonly OfficeDbContext _context;
        public UserService(OfficeDbContext context)
        {
            _context = context;
        }

        public async Task<User> Register(RegisterRequest request)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                LoginId = request.LoginId,
                PasswordHash = hashedPassword,        // ← 평문? 해시값? 뭘 넣어야 하지
                Name = request.Name,
                EmployeeNumber = request.EmployeeNumber,      // ← request의 뭘 연결?
                RoleId = 1    
                // ← 기본 role. 아까 시드 넣은 거 중 "일반직원"의 Id가 몇이지?
            };
                        _context.Users.Add(user);          // 저장할 준비 (아직 DB 안 감)
            await _context.SaveChangesAsync();  // 실제 INSERT 실행 (여기서 DB 감)
            return user;

        }
        public async Task<User?> Login(LoginRequest login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.LoginId == login.LoginId);
            if (user == null)
            {
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }
    }
}
