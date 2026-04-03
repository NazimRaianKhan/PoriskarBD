using Microsoft.EntityFrameworkCore;
using PoriskarBD.Data;
using PoriskarBD.DTOs;
using PoriskarBD.Interfaces;
using PoriskarBD.Models;

namespace PoriskarBD.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(string? role)
        {
            var query = _context.Users.Include(u => u.Zone).AsQueryable();

            if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var roleEnum))
                query = query.Where(u => u.Role == roleEnum);

            return await query.Select(u => MapToDto(u)).ToListAsync();
        }

        public async Task<IEnumerable<UserDto>> GetCollectorsAsync()
        {
            return await _context.Users
                .Include(u => u.Zone)
                .Where(u => u.Role == UserRole.Collector)
                .Select(u => MapToDto(u))
                .ToListAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.Include(u => u.Zone).FirstOrDefaultAsync(u => u.Id == id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto?> GetProfileAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Zone).FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.Include(u => u.Zone).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId);
            if (emailExists) return null;

            user.Name = dto.Name;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();
            return MapToDto(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private static UserDto MapToDto(User u) => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role.ToString(),
            ZoneId = u.ZoneId,
            ZoneName = u.Zone?.Name
        };
    }
}
