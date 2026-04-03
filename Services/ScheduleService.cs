using Microsoft.EntityFrameworkCore;
using PoriskarBD.Data;
using PoriskarBD.DTOs;
using PoriskarBD.Interfaces;
using PoriskarBD.Models;

namespace PoriskarBD.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            return await _context.CollectionSchedules
                .Include(s => s.Zone)
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleDto>> GetByZoneAsync(int zoneId)
        {
            return await _context.CollectionSchedules
                .Include(s => s.Zone)
                .Where(s => s.ZoneId == zoneId)
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, ScheduleDto? Data)> CreateAsync(CreateScheduleDto dto)
        {
            if (!await _context.Zones.AnyAsync(z => z.Id == dto.ZoneId))
                return (false, "Zone not found.", null);

            var schedule = new CollectionSchedule
            {
                ZoneId = dto.ZoneId,
                DayOfWeek = (DayOfWeekEnum)dto.DayOfWeek,
                TimeSlot = dto.TimeSlot
            };

            _context.CollectionSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            await _context.Entry(schedule).Reference(s => s.Zone).LoadAsync();
            return (true, "Schedule created.", MapToDto(schedule));
        }

        public async Task<(bool Success, string Message, ScheduleDto? Data)> UpdateAsync(int id, CreateScheduleDto dto)
        {
            var schedule = await _context.CollectionSchedules
                .Include(s => s.Zone)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
                return (false, "Schedule not found.", null);

            if (!await _context.Zones.AnyAsync(z => z.Id == dto.ZoneId))
                return (false, "Zone not found.", null);

            schedule.ZoneId = dto.ZoneId;
            schedule.DayOfWeek = (DayOfWeekEnum)dto.DayOfWeek;
            schedule.TimeSlot = dto.TimeSlot;

            await _context.SaveChangesAsync();
            await _context.Entry(schedule).Reference(s => s.Zone).LoadAsync();

            return (true, "Schedule updated.", MapToDto(schedule));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _context.CollectionSchedules.FindAsync(id);
            if (schedule == null) return false;

            _context.CollectionSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ScheduleDto MapToDto(CollectionSchedule s) => new ScheduleDto
        {
            Id = s.Id,
            ZoneId = s.ZoneId,
            ZoneName = s.Zone?.Name ?? string.Empty,
            DayOfWeek = s.DayOfWeek.ToString(),
            TimeSlot = s.TimeSlot
        };
    }
}