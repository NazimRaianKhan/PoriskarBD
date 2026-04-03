using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWasteManagement.DTOs;
using SmartWasteManagement.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartWasteManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WasteReportsController : ControllerBase
    {
        private readonly IWasteReportService _reportService;

        public WasteReportsController(IWasteReportService reportService)
        {
            _reportService = reportService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string GetUserRole() =>
            User.FindFirstValue(ClaimTypes.Role)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _reportService.GetAllAsync(GetUserId(), GetUserRole());
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var report = await _reportService.GetByIdAsync(id, GetUserId(), GetUserRole());
            if (report == null) return NotFound(new { message = "Report not found or access denied." });
            return Ok(report);
        }

        [HttpPost]
        [Authorize(Roles = "Citizen")]
        public async Task<IActionResult> Create([FromBody] CreateWasteReportDto dto)
        {
            var report = await _reportService.CreateAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
        }

        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignCollectorDto dto)
        {
            var (success, message, data) = await _reportService.AssignCollectorAsync(id, dto.CollectorId);
            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        [HttpPatch("{id}/collect")]
        [Authorize(Roles = "Collector")]
        public async Task<IActionResult> MarkCollected(int id)
        {
            var (success, message, data) = await _reportService.MarkCollectedAsync(id, GetUserId());
            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FilterByStatus([FromQuery] string status)
        {
            var reports = await _reportService.FilterByStatusAsync(status);
            return Ok(reports);
        }
    }
}