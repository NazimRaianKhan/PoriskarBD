using PoriskarBD.Models;

namespace PoriskarBD.DTOs
{
    // ── Auth ──────────────────────────────────────────────────────────────────
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Citizen;
        public int? ZoneId { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // ── User ──────────────────────────────────────────────────────────────────
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? ZoneId { get; set; }
        public string? ZoneName { get; set; }
    }

    // ── Zone ──────────────────────────────────────────────────────────────────
    public class CreateZoneDto
    {
        public string Name { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
    }

    public class ZoneDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
    }

    // ── WasteReport ───────────────────────────────────────────────────────────
    public class CreateWasteReportDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class WasteReportDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CitizenId { get; set; }
        public string CitizenName { get; set; } = string.Empty;
        public int? CollectorId { get; set; }
        public string? CollectorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AssignCollectorDto
    {
        public int CollectorId { get; set; }
    }

    // ── Schedule ──────────────────────────────────────────────────────────────
    public class CreateScheduleDto
    {
        public int ZoneId { get; set; }
        public int DayOfWeek { get; set; }  
        public string TimeSlot { get; set; } = string.Empty;
    }

    public class ScheduleDto
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
    }

    // ── CollectionLog ─────────────────────────────────────────────────────────
    public class CollectionLogDto
    {
        public int Id { get; set; }
        public int WasteReportId { get; set; }
        public string ReportTitle { get; set; } = string.Empty;
        public int CollectorId { get; set; }
        public string CollectorName { get; set; } = string.Empty;
        public DateTime CollectedAt { get; set; }
    }

    // ── Admin Stats ───────────────────────────────────────────────────────────
    public class AdminStatsDto
    {
        public int TotalReports { get; set; }
        public int ReportedCount { get; set; }
        public int AssignedCount { get; set; }
        public int CollectedCount { get; set; }
        public int TotalZones { get; set; }
        public int TotalCollectors { get; set; }
        public int TotalCitizens { get; set; }
    }
}
