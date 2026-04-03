using Microsoft.EntityFrameworkCore;
using PoriskarBD.Data;
using PoriskarBD.DTOs;
using PoriskarBD.Interfaces;
using PoriskarBD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoriskarBD.Services
{
    public class WasteReportService : IWasteReportService
    {
        private readonly AppDbContext _context;

        public WasteReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WasteReportDto>> GetAllAsync(int userId, string role)
        {
            var query = _context.WasteReports
                .Include(r => r.Citizen)
                .Include(r => r.Collector)
                .AsQueryable();

            // Citizens only see their own reports; Collectors only see assigned ones
            if (role == "Citizen")
                query = query.Where(r => r.CitizenId == userId);
            else if (role == "Collector")
                query = query.Where(r => r.CollectorId == userId);

            var reports = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
            return reports.Select(MapToDto);
        }

        public async Task<WasteReportDto?> GetByIdAsync(int id, int userId, string role)
        {
            var report = await _context.WasteReports
                .Include(r => r.Citizen)
                .Include(r => r.Collector)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return null;

            // Enforce ownership rules
            if (role == "Citizen" && report.CitizenId != userId) return null;
            if (role == "Collector" && report.CollectorId != userId) return null;

            return MapToDto(report);
        }

        public async Task<WasteReportDto> CreateAsync(CreateWasteReportDto dto, int citizenId)
        {
            var report = new WasteReport
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                CitizenId = citizenId,
                Status = ReportStatus.Reported,
                CreatedAt = DateTime.UtcNow
            };

            _context.WasteReports.Add(report);
            await _context.SaveChangesAsync();

            // Re-fetch with navigation properties for the response
            var created = await _context.WasteReports
                .Include(r => r.Citizen)
                .Include(r => r.Collector)
                .FirstAsync(r => r.Id == report.Id);

            return MapToDto(created);
        }

        public async Task<(bool Success, string Message, WasteReportDto? Data)> AssignCollectorAsync(
            int reportId, int collectorId)
        {
            var report = await _context.WasteReports
                .Include(r => r.Citizen)
                .Include(r => r.Collector)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return (false, "Report not found.", null);

            if (report.Status == ReportStatus.Collected)
                return (false, "Report is already collected.", null);

            var collector = await _context.Users.FirstOrDefaultAsync(
                u => u.Id == collectorId && u.Role == UserRole.Collector);

            if (collector == null)
                return (false, "Collector not found.", null);

            report.CollectorId = collectorId;
            report.Collector = collector;
            report.Status = ReportStatus.Assigned;

            await _context.SaveChangesAsync();
            return (true, "Collector assigned.", MapToDto(report));
        }

        public async Task<(bool Success, string Message, WasteReportDto? Data)> MarkCollectedAsync(
            int reportId, int collectorId)
        {
            var report = await _context.WasteReports
                .Include(r => r.Citizen)
                .Include(r => r.Collector)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return (false, "Report not found.", null);

            if (report.CollectorId != collectorId)
                return (false, "You are not assigned to this report.", null);

            if (report.Status != ReportStatus.Assigned)
                return (false, "Report must be in Assigned status to mark as collected.", null);

            report.Status = ReportStatus.Collected;

            var log = new CollectionLog
            {
                WasteReportId = report.Id,
                CollectorId = collectorId,
                CollectedAt = DateTime.UtcNow
            };

            _context.CollectionLogs.Add(log);
            await _context.SaveChangesAsync();

            return (true, "Marked as collected.", MapToDto(report));
        }

        public async Task<IEnumerable<WasteReportDto>> FilterByStatusAsync(string status)
        {
            if (!Enum.TryParse<ReportStatus>(status, true, out var statusEnum))
                return Enumerable.Empty<WasteReportDto>();

            var reports = await _context.WasteReports
                .Include(r => r.Citizen)
                .Include(r => r.Collector)
                .Where(r => r.Status == statusEnum)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reports.Select(MapToDto);
        }

        private static WasteReportDto MapToDto(WasteReport r) => new WasteReportDto
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            Location = r.Location,
            Status = r.Status.ToString(),
            CitizenId = r.CitizenId,
            CitizenName = r.Citizen?.Name ?? string.Empty,
            CollectorId = r.CollectorId,
            CollectorName = r.Collector?.Name,
            CreatedAt = r.CreatedAt
        };
    }
}