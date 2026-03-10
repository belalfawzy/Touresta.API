using Microsoft.AspNetCore.Mvc;
using RAFIQ.API.Data;
using RAFIQ.API.DTOs.Auth;
using RAFIQ.API.Services.Interfaces;

namespace RAFIQ.API.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Admin Authentication")]
    [ApiExplorerSettings(GroupName = "admin")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly AppDbContext _db;

        public AdminAuthController(IAuthService auth, AppDbContext db)
        {
            _auth = auth;
            _db = db;
        }

        /// <summary>
        /// Check if an admin email exists.
        /// </summary>
        /// <param name="request">The email to check.</param>
        /// <response code="200">Email exists in the system.</response>
        /// <response code="400">Email is missing or invalid.</response>
        /// <response code="404">Email not found.</response>
        [HttpPost("check-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult CheckAdminEmail([FromBody] EmailRequest request)
        {
            var (success, message, admin) = _auth.CheckAdminEmail(request.Email);

            if (!success)
                return NotFound(new
                {
                    message,
                    action = "stay_on_email_page"
                });

            return Ok(new
            {
                message,
                action = "go_to_password_page",
                email = request.Email
            });
        }

        /// <summary>
        /// Verify admin password and send OTP.
        /// </summary>
        /// <param name="request">Admin login credentials.</param>
        /// <response code="200">Password verified, OTP sent to email.</response>
        /// <response code="400">Invalid credentials or missing fields.</response>
        [HttpPost("verify-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyAdminPassword([FromBody] LoginRequest request)
        {
            var (success, message, otp) = await _auth.AdminPasswordLoginWithOtpAsync(request.Email, request.Password);

            if (!success)
                return BadRequest(new
                {
                    message,
                    action = "stay_on_password_page"
                });

            return Ok(new
            {
                message,
                action = "go_to_otp_page",
                email = request.Email,
                debugOtp = otp
            });
        }

        /// <summary>
        /// Admin Google login - sends OTP to email.
        /// </summary>
        /// <param name="request">Admin email for Google login.</param>
        /// <response code="200">OTP sent to admin email.</response>
        /// <response code="400">Email is missing.</response>
        /// <response code="401">Email not authorized as admin.</response>
        [HttpPost("google-login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GoogleAdminLogin([FromBody] EmailRequest request)
        {
            var result = await _auth.GoogleAdminLogin(request.Email);

            if (!result.Success)
                return Unauthorized(new
                {
                    result.Message,
                    action = "stay_on_google_login"
                });

            return Ok(new
            {
                message = result.Message,
                action = "go_to_otp_page",
                email = request.Email,
                debugOtp = result.Otp
            });
        }

        /// <summary>
        /// Verify admin OTP and get JWT token.
        /// </summary>
        /// <param name="request">Email and OTP code.</param>
        /// <response code="200">OTP verified, JWT token returned with admin data.</response>
        /// <response code="400">Invalid or expired OTP.</response>
        [HttpPost("verify-otp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult VerifyAdminOtp([FromBody] VerifyCodeRequest request)
        {
            var (success, token, message) = _auth.VerifyAdminOtp(request.Email, request.Code);

            if (!success)
                return BadRequest(new
                {
                    message,
                    action = "stay_on_otp_page"
                });

            var admin = _db.Admins.SingleOrDefault(a => a.Email == request.Email && a.IsActive);
            if (admin == null)
            {
                return StatusCode(500, new { message = "Admin data not found after OTP verification" });
            }

            return Ok(new
            {
                token,
                message,
                action = "go_to_dashboard",
                user = new
                {
                    id = admin.Id,
                    fullName = admin.FullName,
                    email = admin.Email,
                    role = admin.Role.ToString(),
                    type = "admin",
                    createdAt = admin.CreatedAt
                }
            });
        }
    }
}
