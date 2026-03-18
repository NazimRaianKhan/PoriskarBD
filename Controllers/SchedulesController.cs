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
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        // GET: api/schedules/zone/{zoneId}
        [HttpGet("zone/{zoneId}")]
        public async Task<IActionResult> GetSchedulesByZone(int zoneId)
        {
            var schedules = await _scheduleService.GetSchedulesByZoneAsync(zoneId);
            return Ok(schedules);
        }

        // POST: api/schedules
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto createScheduleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSchedule = await _scheduleService.CreateScheduleAsync(createScheduleDto);
            return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.Id }, createdSchedule);
        }

        // GET: api/schedules/{id} (helper for CreatedAtAction)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound($"Schedule with ID {id} not found.");
            return Ok(schedule);
        }

        // PUT: api/schedules/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDto updateScheduleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedSchedule = await _scheduleService.UpdateScheduleAsync(id, updateScheduleDto);
            if (updatedSchedule == null)
                return NotFound($"Schedule with ID {id} not found.");

            return Ok(updatedSchedule);
        }

        // DELETE: api/schedules/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var deleted = await _scheduleService.DeleteScheduleAsync(id);
            if (!deleted)
                return NotFound($"Schedule with ID {id} not found.");

            return NoContent();
        }
    }
}