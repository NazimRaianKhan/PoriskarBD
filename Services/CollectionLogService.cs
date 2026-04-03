using Microsoft.EntityFrameworkCore;
using PoriskarBD.Data;
using PoriskarBD.DTOs;
using PoriskarBD.Interfaces;
using PoriskarBD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoriskarBD.Services
{
    public class CollectionLogService : ICollectionLogService
    {
        private readonly AppDbContext _context;

        public CollectionLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CollectionLogDto>> GetAllAsync()
        {
            return await _context.CollectionLogs
                .Include(l => l.WasteReport)
                .Include(l => l.Collector)
                .OrderByDescending(l => l.CollectedAt)
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<IEnumerable<CollectionLogDto>> GetByCollectorAsync(int collectorId)
        {
            return await _context.CollectionLogs
                .Include(l => l.WasteReport)
                .Include(l => l.Collector)
                .Where(l => l.CollectorId == collectorId)
                .OrderByDescending(l => l.CollectedAt)
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        private static CollectionLogDto MapToDto(CollectionLog l) => new CollectionLogDto
        {
            Id = l.Id,
            WasteReportId = l.WasteReportId,
            ReportTitle = l.WasteReport?.Title ?? string.Empty,
            CollectorId = l.CollectorId,
            CollectorName = l.Collector?.Name ?? string.Empty,
            CollectedAt = l.CollectedAt
        };
    }

    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminStatsDto> GetStatsAsync()
        {
            return new AdminStatsDto
            {
                TotalReports = await _context.WasteReports.CountAsync(),
                ReportedCount = await _context.WasteReports.CountAsync(r => r.Status == ReportStatus.Reported),
                AssignedCount = await _context.WasteReports.CountAsync(r => r.Status == ReportStatus.Assigned),
                CollectedCount = await _context.WasteReports.CountAsync(r => r.Status == ReportStatus.Collected),
                TotalZones = await _context.Zones.CountAsync(),
                TotalCollectors = await _context.Users.CountAsync(u => u.Role == UserRole.Collector),
                TotalCitizens = await _context.Users.CountAsync(u => u.Role == UserRole.Citizen)
            };
        }

        public async Task<IEnumerable<object>> GetZoneSummaryAsync()
        {
            var zones = await _context.Zones.ToListAsync();
            var summary = new List<object>();

            foreach (var zone in zones)
            {
                var citizenIds = await _context.Users
                    .Where(u => u.ZoneId == zone.Id && u.Role == UserRole.Citizen)
                    .Select(u => u.Id)
                    .ToListAsync();

                var totalReports = await _context.WasteReports.CountAsync(r => citizenIds.Contains(r.CitizenId));
                var collectedReports = await _context.WasteReports.CountAsync(
                    r => citizenIds.Contains(r.CitizenId) && r.Status == ReportStatus.Collected);

                summary.Add(new
                {
                    ZoneId = zone.Id,
                    ZoneName = zone.Name,
                    AreaName = zone.AreaName,
                    TotalReports = totalReports,
                    CollectedReports = collectedReports,
                    PendingReports = totalReports - collectedReports
                });
            }

            return summary;
        }
    }
}