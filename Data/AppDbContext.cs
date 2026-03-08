using Microsoft.EntityFrameworkCore;
using PoriskarBD.Models;

namespace PoriskarBD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<WasteReport> WasteReports { get; set; }
        public DbSet<CollectionSchedule> CollectionSchedules { get; set; }
        public DbSet<CollectionLog> CollectionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Citizen → WasteReport
            modelBuilder.Entity<WasteReport>()
                .HasOne(r => r.Citizen)
                .WithMany(u => u.ReportedIssues)
                .HasForeignKey(r => r.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Collector → WasteReport
            modelBuilder.Entity<WasteReport>()
                .HasOne(r => r.Collector)
                .WithMany(u => u.AssignedReports)
                .HasForeignKey(r => r.CollectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // CollectionLog → WasteReport
            modelBuilder.Entity<CollectionLog>()
                .HasOne(l => l.WasteReport)
                .WithOne(r => r.CollectionLog)
                .HasForeignKey<CollectionLog>(l => l.WasteReportId)
                .OnDelete(DeleteBehavior.Restrict);

            // CollectionLog → Collector
            modelBuilder.Entity<CollectionLog>()
                .HasOne(l => l.Collector)
                .WithMany(u => u.CollectionLogs)
                .HasForeignKey(l => l.CollectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique email constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


        }
    }
}
