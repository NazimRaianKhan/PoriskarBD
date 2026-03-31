using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoriskarBD.Interfaces;
using PoriskarBD.DTOs; // Assuming DTOs exist; adjust namespace as needed
using System.Threading.Tasks;

namespace PoriskarBD.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ZonesController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZonesController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        // GET: api/zones
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var zones = await _zoneService.GetAllAsync();
            return Ok(zones);
        }

        // GET: api/zones/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _zoneService.GetByIdAsync(id);
            if (zone == null) 
                return NotFound(new { message = "Zone not found." });
            return Ok(zone);
        }

        // POST: api/zones
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateZone([FromBody] CreateZoneDto createZoneDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdZone = await _zoneService.CreateZoneAsync(createZoneDto);
            return CreatedAtAction(nameof(GetZoneById), new { id = createdZone.Id }, createdZone);
        }

        // PUT: api/zones/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateZone(int id, [FromBody] UpdateZoneDto updateZoneDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedZone = await _zoneService.UpdateZoneAsync(id, updateZoneDto);
            if (updatedZone == null)
                return NotFound($"Zone with ID {id} not found.");

            return Ok(updatedZone);
        }

        // DELETE: api/zones/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZone(int id)
        {
            var deleted = await _zoneService.DeleteZoneAsync(id);
            if (!deleted)
                return NotFound($"Zone with ID {id} not found.");

            return NoContent();
        }
    }
}