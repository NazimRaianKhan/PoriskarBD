using Microsoft.EntityFrameworkCore;
using PoriskarBD.Data;
using PoriskarBD.DTOs;
using PoriskarBD.Helpers;
using PoriskarBD.Interfaces;
using PoriskarBD.Models;

namespace PoriskarBD.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return (false, "Email is already registered.");

            if (dto.ZoneId.HasValue && !await _context.Zones.AnyAsync(z => z.Id == dto.ZoneId))
                return (false, "Zone not found.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                ZoneId = dto.ZoneId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (true, "Registration successful.");
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return new AuthResponseDto
            {
                Token = _jwtHelper.GenerateToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
