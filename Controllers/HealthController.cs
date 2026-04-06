using Microsoft.AspNetCore.Mvc;
using PoriskarBD.Data;

namespace PoriskarBD.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HealthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //keeps the database connection alive
            var userCount = _db.Users.Count();
            return Ok(new
            {
                status = "healthy",
                time = DateTime.UtcNow,
                database = "connected"
            });
        }
    }

}
