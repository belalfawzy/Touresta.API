using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.DTOs;
using Touresta.API.Models;

namespace Touresta.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly EmailService _emailService;

        public AuthController(AuthService auth, EmailService emailService)
        {
            _auth = auth;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest req)
        {
<<<<<<< HEAD
            var (success, message, user) = await _auth.RegisterAsync(req);
=======
            var (success, message, userId) = await _auth.RegisterAsync(req);
            return success ? Ok(new { message, userId }) : BadRequest(new { message });

        }

        [HttpPost("check-email")]
        public IActionResult CheckEmail([FromBody] EmailRequest req)
        {
            var (success, message) = _auth.CheckEmail(req.Email);
            if (!success) return NotFound(new { message, action = "stay_on_email_page" });
            return Ok(new { message, action = "go_to_password_page", email = req.Email });
        }

        [HttpPost("verify-password")]
        public IActionResult VerifyPassword([FromBody] LoginRequest req)
        {
            var (success, token, message) = _auth.VerifyPassword(req.Email, req.Password);
>>>>>>> eeba5c903ff192eb9dc107ccd442125577d8624c

            if (!success)
                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = message
                });

            var token = _auth.GenerateUserJwtToken(user);

            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Message = message,
                Data = new AuthResponse
                {
                    Token = token,
                    Message = message,
                    Action = "go_to_verification",
                    User = new UserData
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Gender = user.Gender,
                        Country = user.Country,
                        IsVerified = user.IsVerified,
                        Type = "user"
                    }
                }
            });
        }

        [HttpPost("check-email")]
        public ActionResult<ApiResponse<object>> CheckEmail([FromBody] EmailRequest req)
        {
            var (success, message, user) = _auth.CheckEmail(req.Email);

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
                Data = new { email = req.Email, userExists = true }
            });
        }

        [HttpPost("verify-password")]
        public ActionResult<ApiResponse<AuthResponse>> VerifyPassword([FromBody] LoginRequest req)
        {
            var (success, token, message, user) = _auth.VerifyPassword(req.Email, req.Password);

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
                    User = new UserData
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Gender = user.Gender,
                        BirthDate = user.BirthDate,
                        Country = user.Country,
                        IsVerified = user.IsVerified,
                        Type = "user"
                    }
                }
            });
        }

        [HttpPost("send-verification-code")]
        public async Task<ActionResult<ApiResponse<object>>> SendVerificationCode([FromBody] EmailRequest req)
        {
            var (success, message, code) = await _auth.SendVerificationCode(req.Email);

            if (!success)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = message
                });

            // إرسال الإيميل
            if (!string.IsNullOrEmpty(code))
            {
                await _emailService.SendOtpEmail(req.Email, code);
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Verification code sent to email",
                Action = "enter_verification_code",
                Data = new { email = req.Email }
            });
        }

        [HttpPost("verify-code")]
        public ActionResult<ApiResponse<AuthResponse>> VerifyCode([FromBody] VerifyCodeRequest req)
        {
            var (success, token, message, user) = _auth.VerifyCode(req.Email, req.Code);

            if (!success)
                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = message
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
                    User = new UserData
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Gender = user.Gender,
                        BirthDate = user.BirthDate,
                        Country = user.Country,
                        IsVerified = user.IsVerified,
                        Type = "user"
                    }
                }
            });
        }

        [HttpPost("google-signin")]
        public async Task<ActionResult<ApiResponse<object>>> GoogleSignIn([FromBody] GoogleRegisterRequest req)
        {
            // معالجة تسجيل الدخول بـ Google
            var (success, message, userExists, user, code) = await _auth.ProcessGoogleSignIn(req.Email, req.GoogleId, req.Name);

            if (!success)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = message
                });

            // لو اليوزر موجود - نرسل كود التحقق
            if (userExists)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    await _emailService.SendOtpEmail(req.Email, code);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Verification code sent to your email",
                    Action = "go_to_verification",
                    Data = new
                    {
                        email = req.Email,
                        userExists = true,
                        requiresVerification = true
                    }
                });
            }
            // لو اليوزر مش موجود - نروح لصفحة التسجيل
            else
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Please complete your registration",
                    Action = "go_to_register",
                    Data = new
                    {
                        email = req.Email,
                        name = req.Name,
                        googleId = req.GoogleId,
                        userExists = false,
                        requiresRegistration = true
                    }
                });
            }
        }

        [HttpPost("complete-google-registration")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> CompleteGoogleRegistration([FromBody] GoogleRegisterRequest req)
        {
            var (success, token, message, user) = await _auth.RegisterWithGoogleAsync(req);

            if (!success)
                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = message
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
                    User = new UserData
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        IsVerified = true,
                        Type = "user"
                    }
                }
            });
        }


        [HttpPost("verify-google-token")]
        public async Task<ActionResult<ApiResponse<object>>> VerifyGoogleToken([FromBody] GoogleTokenRequest req)
        {
            // استخدام أسماء متغيرات محددة بدل var
            var result = await _auth.VerifyGoogleTokenAsync(req.IdToken);
            bool success = result.Success;
            string? email = result.Email;
            string message = result.Message;
            User? user = result.User;

            if (!success)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = message
                });

            // حل مشكلة الـ Infinity Loop - التحقق من حالة المستخدم بشكل صحيح
            if (user == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Google account not registered",
                    Action = "redirect_to_signup",
                    Data = new { email = email, userExists = false }
                });
            }

            // إذا المستخدم موجود، نرسل كود التحقق
            var codeResult = await _auth.SendVerificationCode(email);
            bool codeSuccess = codeResult.Success;
            string codeMessage = codeResult.Message;
            string? code = codeResult.Code;

            if (codeSuccess && !string.IsNullOrEmpty(code))
            {
                await _emailService.SendOtpEmail(email, code);
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Verification code sent to email",
                Action = "enter_verification_code",
                Data = new
                {
                    email = email,
                    userExists = true,
                    requiresVerification = true
                }
            });
        }
        //حفظ الصوره يا انس اهو 
        [HttpPost("upload-profile-image")]
        [ProducesResponseType(typeof(UploadProfileImageResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile imageFile, [FromForm] string userId)
        {
            if (imageFile == null || string.IsNullOrEmpty(userId))
                return BadRequest(new { success = false, message = "Missing userId or imageFile" });

            // مسار الحفظ
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

            var response = new UploadProfileImageResponse
            {
                Success = true,
                Message = "Profile image uploaded successfully",
                ImageUrl = imageUrl,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }

        public class GoogleTokenRequest
        {
            public string IdToken { get; set; } = string.Empty;
        }
    }
}