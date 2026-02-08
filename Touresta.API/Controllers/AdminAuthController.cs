using Microsoft.AspNetCore.Mvc;
using Touresta.API.Data;
using Touresta.API.DTOs;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Admin authentication endpoints with 2-step OTP verification.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Admin Authentication")]
    public class AdminAuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly AppDbContext _db;

        public AdminAuthController(AuthService auth, AppDbContext db)
        {
            _auth = auth;
            _db = db;
        }

        /// <summary>
        /// Check if an admin email exists.
        /// </summary>
        /// <remarks>
        /// Verifies whether the provided email belongs to an active admin account.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/adminauth/check-email
        ///     {
        ///         "email": "admin@touresta.com"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "Email exists",
        ///         "action": "go_to_password_page",
        ///         "email": "admin@touresta.com"
        ///     }
        ///
        /// **Not Found Response:**
        ///
        ///     {
        ///         "message": "This email is not registered in our system.",
        ///         "action": "stay_on_email_page"
        ///     }
        /// </remarks>
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
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Verify admin password and send OTP.
        /// </summary>
        /// <remarks>
        /// Validates admin credentials and sends a 6-digit OTP to the admin's email for 2-step verification.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/adminauth/verify-password
        ///     {
        ///         "email": "admin@touresta.com",
        ///         "password": "Admin@123"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "OTP sent to admin email",
        ///         "action": "go_to_otp_page",
        ///         "email": "admin@touresta.com",
        ///         "debugOtp": "123456"
        ///     }
        /// </remarks>
        /// <param name="request">Admin login credentials.</param>
        /// <response code="200">Password verified, OTP sent to email.</response>
        /// <response code="400">Invalid credentials or missing fields.</response>
        [HttpPost("verify-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyAdminPassword([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Email and password are required" });
                }

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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Admin Google login - sends OTP to email.
        /// </summary>
        /// <remarks>
        /// Initiates admin login via Google. Sends a 6-digit OTP to the admin's email for verification.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/adminauth/google-login
        ///     {
        ///         "email": "admin@touresta.com"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "OTP sent to admin email",
        ///         "action": "go_to_otp_page",
        ///         "email": "admin@touresta.com",
        ///         "debugOtp": "654321"
        ///     }
        /// </remarks>
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
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Verify admin OTP and get JWT token.
        /// </summary>
        /// <remarks>
        /// Validates the 6-digit OTP sent to admin's email. Returns a JWT token on success.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/adminauth/verify-otp
        ///     {
        ///         "email": "admin@touresta.com",
        ///         "code": "123456"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIs...",
        ///         "message": "Admin verified successfully",
        ///         "action": "go_to_dashboard",
        ///         "user": {
        ///             "id": 1,
        ///             "fullName": "Main Admin",
        ///             "email": "admin@touresta.com",
        ///             "role": "SuperAdmin",
        ///             "type": "admin",
        ///             "createdAt": "2025-01-01T00:00:00Z"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="request">Email and OTP code.</param>
        /// <response code="200">OTP verified, JWT token returned with admin data.</response>
        /// <response code="400">Invalid or expired OTP.</response>
        [HttpPost("verify-otp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult VerifyAdminOtp([FromBody] VerifyCodeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
                {
                    return BadRequest(new { message = "Email and code are required" });
                }

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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
