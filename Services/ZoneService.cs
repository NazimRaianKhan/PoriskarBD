using Microsoft.EntityFrameworkCore;
using PoriskarBD.Data;
using PoriskarBD.DTOs;
using PoriskarBD.Interfaces;
using PoriskarBD.Models;

namespace PoriskarBD.Services
{
    public class ZoneService : IZoneService
    {
        private readonly AppDbContext _context;

        public ZoneService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ZoneDto>> GetAllAsync()
        {
            return await _context.Zones
                .Select(z => MapToDto(z))
                .ToListAsync();
        }

        public async Task<ZoneDto?> GetByIdAsync(int id)
        {
            var zone = await _context.Zones.FindAsync(id);
            return zone == null ? null : MapToDto(zone);
        }

        public async Task<ZoneDto> CreateAsync(CreateZoneDto dto)
        {
            var zone = new Zone { Name = dto.Name, AreaName = dto.AreaName };
            _context.Zones.Add(zone);
            await _context.SaveChangesAsync();
            return MapToDto(zone);
        }

        public async Task<ZoneDto?> UpdateAsync(int id, CreateZoneDto dto)
        {
            var zone = await _context.Zones.FindAsync(id);
            if (zone == null) return null;

            zone.Name = dto.Name;
            zone.AreaName = dto.AreaName;
            await _context.SaveChangesAsync();
            return MapToDto(zone);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var zone = await _context.Zones.FindAsync(id);
            if (zone == null) return false;

            _context.Zones.Remove(zone);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ZoneDto MapToDto(Zone z) => new ZoneDto
        {
            Id = z.Id,
            Name = z.Name,
            AreaName = z.AreaName
        };
    }
}