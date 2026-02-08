using Microsoft.AspNetCore.Mvc;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Health check and status endpoints for the Touresta API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Health Check")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Get API status.
        /// </summary>
        /// <remarks>
        /// Returns a basic health check confirming the API is running.
        ///
        /// **Example Response:**
        ///
        ///     {
        ///         "message": "Touresta API is running",
        ///         "version": "1.0",
        ///         "status": "Active"
        ///     }
        /// </remarks>
        /// <response code="200">API is running.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Touresta API is running",
                version = "1.0",
                status = "Active"
            });
        }

        /// <summary>
        /// Get current server status with timestamp.
        /// </summary>
        /// <remarks>
        /// Returns the current status and UTC timestamp.
        ///
        /// **Example Response:**
        ///
        ///     {
        ///         "status": "OK",
        ///         "timestamp": "2025-01-15T12:00:00Z"
        ///     }
        /// </remarks>
        /// <response code="200">Server status returned.</response>
        [HttpGet("status")]
        [ProducesResponseType(200)]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "OK", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Get application info.
        /// </summary>
        /// <remarks>
        /// Returns application name, version, and environment.
        ///
        /// **Example Response:**
        ///
        ///     {
        ///         "app": "Touresta API",
        ///         "version": "1.0",
        ///         "environment": "Production"
        ///     }
        /// </remarks>
        /// <response code="200">App info returned.</response>
        [HttpGet("info")]
        [ProducesResponseType(200)]
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
