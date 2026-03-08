namespace PoriskarBD.Models
{
    public enum UserRole { Citizen, Collector, Admin }
    public enum ReportStatus { Reported, Assigned, Collected }
    public enum DayOfWeekEnum { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int? ZoneId { get; set; }    //admin
        public Zone? Zone { get; set; }
        public ICollection<WasteReport> ReportedIssues { get; set; } = new List<WasteReport>();
        public ICollection<WasteReport> AssignedReports { get; set; } = new List<WasteReport>();
        public ICollection<CollectionLog> CollectionLogs { get; set; } = new List<CollectionLog>();
    }

    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<CollectionSchedule> Schedules { get; set; } = new List<CollectionSchedule>();
    }

    public class WasteReport
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ReportStatus Status { get; set; } = ReportStatus.Reported;
        public int CitizenId { get; set; }
        public User? Citizen { get; set; }
        public int? CollectorId { get; set; }
        public User? Collector { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public CollectionLog? CollectionLog { get; set; }
    }

    public class CollectionSchedule
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public Zone? Zone { get; set; }
        public DayOfWeekEnum DayOfWeek { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
    }

    public class CollectionLog
    {
        public int Id { get; set; }
        public int WasteReportId { get; set; }
        public WasteReport? WasteReport { get; set; }
        public int CollectorId { get; set; }
        public User? Collector { get; set; }
        public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
    }
}
