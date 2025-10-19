using Microsoft.AspNetCore.Mvc;

namespace Touresta.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Touresta API is running",
                version = "1.0",
                status = "Active"
            });
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "OK", timestamp = DateTime.UtcNow });
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                app = "Touresta API",
                version = "1.0",
                environment = "Production"
            });
        }
    }
}