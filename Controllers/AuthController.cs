using Microsoft.AspNetCore.Mvc;
using PoriskarBD.DTOs;
using PoriskarBD.Interfaces;

namespace PoriskarBD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, message) = await _authService.RegisterAsync(dto);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null) return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }
        [HttpGet("hash-test")]
        public IActionResult HashTest()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            return Ok(new { hash });
        }
    }
}
