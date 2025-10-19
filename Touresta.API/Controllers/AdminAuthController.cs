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
                Console.WriteLine($"=== CheckAdminEmail Method Called ===");
                Console.WriteLine($"Email: {request?.Email}");

                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                if (string.IsNullOrEmpty(request.Email))
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
                Console.WriteLine($"Error in CheckAdminEmail: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
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
                Console.WriteLine($"Error in VerifyAdminPassword: {ex.Message}");
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
                Console.WriteLine($"Error in GoogleAdminLogin: {ex.Message}");
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
                Console.WriteLine($"Error in VerifyAdminOtp: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("test-real-email")]
        public async Task<IActionResult> TestRealEmail([FromBody] EmailRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

                // نستخدم EmailService مباشرة للتست
                var emailService = new EmailService(_auth.GetConfiguration());
                var testOtp = new Random().Next(100000, 999999).ToString();

                var result = await emailService.SendOtpEmail(request.Email, testOtp);

                if (result)
                {
                    return Ok(new
                    {
                        message = "Test email sent successfully! Check your inbox.",
                        email = request.Email,
                        otp = testOtp
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Failed to send test email. Check email settings and password."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "💥 Error sending test email",
                    error = ex.Message
                });
            }
        }
        [HttpPost("debug-email-settings")]
        public IActionResult DebugEmailSettings()
        {
            try
            {
                var emailService = new EmailService(_auth.GetConfiguration());

                var settings = new
                {
                    SmtpServer = _auth.GetConfiguration()["EmailSettings:SmtpServer"],
                    Port = _auth.GetConfiguration()["EmailSettings:Port"],
                    SenderEmail = _auth.GetConfiguration()["EmailSettings:SenderEmail"],
                    SenderName = _auth.GetConfiguration()["EmailSettings:SenderName"],
                    PasswordLength = _auth.GetConfiguration()["EmailSettings:Password"]?.Length
                };

                return Ok(new
                {
                    message = "Email settings retrieved",
                    settings = settings,
                    note = "Check if password is 16 characters (App Password)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

}