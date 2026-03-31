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
        public async Task<IActionResult> Create([FromBody] CreateZoneDto dto)
        {
            var zone = await _zoneService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
        }

        // PUT: api/zones/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateZoneDto dto)
        {
            var zone = await _zoneService.UpdateAsync(id, dto);
            if (zone == null) return NotFound(new { message = "Zone not found." });
            return Ok(zone);
        }

        // DELETE: api/zones/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _zoneService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "Zone not found." });
            return Ok(new { message = "Zone deleted." });
        }
    }
}