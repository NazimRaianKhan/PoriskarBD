using PoriskarBD.DTOs;

namespace PoriskarBD.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }

    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync(string? role);
        Task<IEnumerable<UserDto>> GetCollectorsAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetProfileAsync(int userId);
        Task<bool> DeleteAsync(int id);
    }

    public interface IZoneService
    {
        Task<IEnumerable<ZoneDto>> GetAllAsync();
        Task<ZoneDto?> GetByIdAsync(int id);
        Task<ZoneDto> CreateAsync(CreateZoneDto dto);
        Task<ZoneDto?> UpdateAsync(int id, CreateZoneDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public interface IWasteReportService
    {
        Task<IEnumerable<WasteReportDto>> GetAllAsync(int userId, string role);
        Task<WasteReportDto?> GetByIdAsync(int id, int userId, string role);
        Task<WasteReportDto> CreateAsync(CreateWasteReportDto dto, int citizenId);
        Task<(bool Success, string Message, WasteReportDto? Data)> AssignCollectorAsync(int reportId, int collectorId);
        Task<(bool Success, string Message, WasteReportDto? Data)> MarkCollectedAsync(int reportId, int collectorId);
        Task<IEnumerable<WasteReportDto>> FilterByStatusAsync(string status);
    }

    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllAsync();
        Task<IEnumerable<ScheduleDto>> GetByZoneAsync(int zoneId);
        Task<(bool Success, string Message, ScheduleDto? Data)> CreateAsync(CreateScheduleDto dto);
        Task<(bool Success, string Message, ScheduleDto? Data)> UpdateAsync(int id, CreateScheduleDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public interface ICollectionLogService
    {
        Task<IEnumerable<CollectionLogDto>> GetAllAsync();
        Task<IEnumerable<CollectionLogDto>> GetByCollectorAsync(int collectorId);
    }

    public interface IAdminService
    {
        Task<AdminStatsDto> GetStatsAsync();
        Task<IEnumerable<object>> GetZoneSummaryAsync();
    }
}
