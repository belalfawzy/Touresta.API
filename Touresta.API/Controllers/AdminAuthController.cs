using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs;

namespace Touresta.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AdminAuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("check-email")]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("verify-password")]
        public IActionResult VerifyAdminPassword([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Email and password are required" });
                }

                var (success, token, message) = _auth.VerifyAdminPassword(request.Email, request.Password);

                if (!success)
                    return BadRequest(new
                    {
                        message,
                        action = "stay_on_password_page"
                    });

                return Ok(new
                {
                    token,
                    message,
                    action = "go_to_dashboard",
                    user = new
                    {
                        email = request.Email,
                        type = "admin"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("google-login")]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("verify-otp")]
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

                return Ok(new
                {
                    token,
                    message,
                    action = "go_to_dashboard",
                    user = new
                    {
                        email = request.Email,
                        type = "admin"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}