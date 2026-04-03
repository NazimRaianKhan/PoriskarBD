using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoriskarBD.Interfaces;
using PoriskarBD.DTOs;
using System.Threading.Tasks;

namespace PoriskarBD.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: api/schedules
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _scheduleService.GetAllAsync();
            return Ok(schedules);
        }

        // GET: api/schedules/zone/{zoneId}
        [HttpGet("zone/{zoneId}")]
        public async Task<IActionResult> GetByZone(int zoneId)
        {
            var schedules = await _scheduleService.GetByZoneAsync(zoneId);
            return Ok(schedules);
        }

        // POST: api/schedules
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            var (success, message, data) = await _scheduleService.CreateAsync(dto);
            if (!success) return BadRequest(new { message });
            return CreatedAtAction(nameof(GetAll), new { }, data);
        }

        // GET: api/schedules/{id} (helper for CreatedAtAction)
        [HttpGet("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateScheduleDto dto)
        {
            var (success, message, data) = await _scheduleService.UpdateAsync(id, dto);
            if (!success) return BadRequest(new { message });
            return Ok(data);
        }


        // DELETE: api/schedules/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _scheduleService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "Schedule not found." });
            return Ok(new { message = "Schedule deleted." });
        }
    }
}