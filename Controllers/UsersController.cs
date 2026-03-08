using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoriskarBD.Interfaces;
using System.Security.Claims;

namespace PoriskarBD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? role)
        {
            var users = await _userService.GetAllAsync(role);
            return Ok(users);
        }

        [HttpGet("collectors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCollectors()
        {
            var collectors = await _userService.GetCollectorsAsync();
            return Ok(collectors);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _userService.GetProfileAsync(GetUserId());
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "User not found." });
            return Ok(new { message = "User deleted." });
        }
    }
}
