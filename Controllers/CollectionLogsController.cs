using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWasteManagement.Interfaces;
using System.Threading.Tasks;

namespace SmartWasteManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CollectionLogsController : ControllerBase
    {
        private readonly ICollectionLogService _logService;

        public CollectionLogsController(ICollectionLogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _logService.GetAllAsync();
            return Ok(logs);
        }

        [HttpGet("collector/{collectorId}")]
        [Authorize(Roles = "Admin,Collector")]
        public async Task<IActionResult> GetByCollector(int collectorId)
        {
            var logs = await _logService.GetByCollectorAsync(collectorId);
            return Ok(logs);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _adminService.GetStatsAsync();
            return Ok(stats);
        }

        [HttpGet("zone-summary")]
        public async Task<IActionResult> GetZoneSummary()
        {
            var summary = await _adminService.GetZoneSummaryAsync();
            return Ok(summary);
        }
    }
}