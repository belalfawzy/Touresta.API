using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs;

namespace Touresta.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly EmailService _emailService;

        public AdminAuthController(AuthService auth, EmailService emailService)
        {
            _auth = auth;
            _emailService = emailService;
        }

        [HttpPost("check-email")]
        public ActionResult<ApiResponse<object>> CheckAdminEmail([FromBody] EmailRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Email is required"
                    });
                }

                var (success, message, admin) = _auth.CheckAdminEmail(request.Email);

                if (!success)
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = message,
                        Action = "stay_on_email_page"
                    });

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = message,
                    Action = "go_to_password_page",
                    Data = new { email = request.Email, adminExists = true }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPost("verify-password")]
        public ActionResult<ApiResponse<AuthResponse>> VerifyAdminPassword([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Message = "Email and password are required"
                    });
                }

                var (success, token, message, admin) = _auth.VerifyAdminPassword(request.Email, request.Password);

                if (!success)
                    return Unauthorized(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Message = message,
                        Action = "stay_on_password_page"
                    });

                return Ok(new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Message = message,
                    Token = token,
                    Action = "go_to_dashboard",
                    Data = new AuthResponse
                    {
                        Token = token,
                        Message = message,
                        Action = "go_to_dashboard",
                        Admin = new AdminData
                        {
                            Id = admin.Id,
                            Email = admin.Email,
                            FullName = admin.FullName,
                            Role = admin.Role.ToString(),
                            Type = "admin"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPost("send-verification-code")]
        public async Task<ActionResult<ApiResponse<object>>> SendAdminVerificationCode([FromBody] EmailRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Email is required"
                    });
                }

                var (success, message, otp) = await _auth.SendAdminVerificationCode(request.Email);

                if (!success)
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = message,
                        Action = "stay_on_google_login"
                    });

                // إرسال الإيميل
                if (!string.IsNullOrEmpty(otp))
                {
                    await _emailService.SendOtpEmail(request.Email, otp);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "OTP sent to your email",
                    Action = "go_to_otp_page",
                    Data = new { email = request.Email }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPost("verify-otp")]
        public ActionResult<ApiResponse<AuthResponse>> VerifyAdminOtp([FromBody] VerifyCodeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
                {
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Message = "Email and code are required"
                    });
                }

                var (success, token, message, admin) = _auth.VerifyAdminOtp(request.Email, request.Code);

                if (!success)
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Message = message,
                        Action = "stay_on_otp_page"
                    });

                return Ok(new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Message = message,
                    Token = token,
                    Action = "go_to_dashboard",
                    Data = new AuthResponse
                    {
                        Token = token,
                        Message = message,
                        Action = "go_to_dashboard",
                        Admin = new AdminData
                        {
                            Id = admin.Id,
                            Email = admin.Email,
                            FullName = admin.FullName,
                            Role = admin.Role.ToString(),
                            Type = "admin"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }
    }
}