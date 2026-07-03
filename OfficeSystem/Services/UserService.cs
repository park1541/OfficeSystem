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
        private readonly IJwtService _jwtService;
        public UserService(OfficeDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<User?> Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.LoginId == request.LoginId))
                return null;
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
        public async Task<LoginResult?> Login(LoginRequest login)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.LoginId == login.LoginId);
            if (user == null)
            {
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return null;
            }
            var token = _jwtService.GenerateToken(user);
           
            return new LoginResult
            {
                User = user,
                Token = token
            };
        }
    }
}
