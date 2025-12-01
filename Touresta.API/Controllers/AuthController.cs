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
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;

        public AuthController(AuthService auth, AppDbContext db, EmailService emailService)
        {
            _auth = auth;
            _db = db;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var (success, message, userId) = await _auth.RegisterAsync(req);

            if (!success)
                return BadRequest(new { message });

          
            var user = await _db.Users.SingleOrDefaultAsync(u => u.UserId == userId);

            return Ok(new
            {
                message,
                userId,
                profileImage = user?.ProfileImageUrl ?? "/images/users/default.png",
                action = "enter_verification_code" 
            });
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

            if (!success)
            {
                return BadRequest(new
                {
                    message,
                    action = "email_not_verified",   // دي الجديدة المهمة اللي هيخلي انس يشوف ان الاكشن هنا مش فريفاي
                    email = req.Email
                });
            }

           
            var user = _db.Users.SingleOrDefault(u => u.Email == req.Email);
            if (user == null)
                return BadRequest(new { message = "User not found after login" });

            return Ok(new
            {
                token,
                message,
                action = "go_to_dashboard",
                user = new
                {
                    id = user.Id,
                    userId = user.UserId,
                    email = user.Email,
                    userName = user.UserName,
                    phoneNumber = user.PhoneNumber,
                    gender = user.Gender,
                    country = user.Country,
                    birthDate = user.BirthDate,
                    profileImageUrl = user.ProfileImageUrl
                }
            });
        }



        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] EmailRequest req)
        {
            var (success, message, code) = _auth.GoogleLogin(req.Email);
            if (!success) return NotFound(new { message });

            // إرسال الإيميل باستخدام الـ EmailService
            if (!string.IsNullOrEmpty(code))
            {
                await _emailService.SendOtpEmail(req.Email, code);
            }

            return Ok(new
            {
                message = "Verification code sent to email",
            });
        }

        [HttpPost("verify-code")]
        public IActionResult VerifyCode([FromBody] VerifyCodeRequest req)
        {
            var (success, token, message) = _auth.VerifyCode(req.Email, req.Code);
            if (!success) return BadRequest(new { message });
            return Ok(new { token, message });
        }


        [HttpPost("google-register")]
        public async Task<IActionResult> RegisterWithGoogle([FromBody] GoogleRegisterRequest req)
        {
            var (success, token, message) = await _auth.RegisterWithGoogleAsync(req);

            if (!success)
                return BadRequest(new { message });

            return Ok(new
            {
                token,
                message,
                action = "go_to_dashboard",
                user = new { email = req.Email, type = "user" }
            });
        }

        [HttpPost("google-verify-code")]
        public IActionResult VerifyGoogleCode([FromBody] VerifyCodeRequest req)
        {
            var (success, token, message) = _auth.VerifyGoogleCode(req.Email, req.Code);

            if (!success)
                return BadRequest(new { message });

            return Ok(new
            {
                token,
                message,
                action = "go_to_dashboard",
                user = new { email = req.Email, type = "user" }
            });
        }


        [HttpPost("verify-google-token")]
        public async Task<IActionResult> VerifyGoogleToken([FromBody] GoogleTokenRequest req)
        {
            var (success, email, message) = await _auth.VerifyGoogleAccountAsync(req.IdToken);

            if (!success)
                return BadRequest(new { message });

            var userExists = _db.Users.Any(u => u.Email == email);

            if (!userExists)
                return NotFound(new
                {
                    message = "Google account not registered",
                    action = "redirect_to_signup",
                    email = email
                });

            var user = _db.Users.SingleOrDefault(u => u.Email == email);
            var code = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = code;
            _db.SaveChanges();

            await _emailService.SendOtpEmail(email, code);

            return Ok(new
            {
                message = "Verification code sent to email",
                action = "enter_verification_code",
                email = email
            });
        }


        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailRequest req)
        {
            if (string.IsNullOrEmpty(req.Email))
                return BadRequest(new { message = "Email is required" });

            var (success, message) = await _auth.SendForgotPasswordCodeAsync(req.Email);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            if (string.IsNullOrEmpty(req.Email) ||
                string.IsNullOrEmpty(req.Code) ||
                string.IsNullOrEmpty(req.NewPassword))
            {
                return BadRequest(new { message = "Email, code and newPassword are required" });
            }

            var (success, message) = await _auth.ResetPasswordAsync(req.Email, req.Code, req.NewPassword);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }



        // تعديل بيانات البروفايل + الصورة يا انس 
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromForm] UpdateProfileRequest request,
            [FromForm] IFormFile? profileImage)
        {
            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest(new { success = false, message = "UserId is required" });

            
            var user = await _db.Users.SingleOrDefaultAsync(u => u.UserId == request.UserId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

         
            if (!string.IsNullOrWhiteSpace(request.UserName))
                user.UserName = request.UserName;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.Country))
                user.Country = request.Country;

            if (!string.IsNullOrWhiteSpace(request.Gender))
                user.Gender = request.Gender;

            if (request.BirthDate.HasValue)
                user.BirthDate = request.BirthDate;

            if (profileImage != null && profileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                
                user.ProfileImageUrl = $"/images/users/{fileName}";
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Profile updated successfully",
                user = new
                {
                    userId = user.UserId,
                    email = user.Email,
                    userName = user.UserName,
                    phoneNumber = user.PhoneNumber,
                    country = user.Country,
                    gender = user.Gender,
                    birthDate = user.BirthDate,
                    profileImage = user.ProfileImageUrl
                }
            });
        }


        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode([FromBody] EmailRequest req)
        {
            var (success, message) = await _auth.ResendVerificationCodeAsync(req.Email);

            if (!success)
                return BadRequest(new { message });

            return Ok(new
            {
                message,
                action = "enter_verification_code",
                email = req.Email
            });
        }

        public class GoogleTokenRequest
        {
            public string IdToken { get; set; } = string.Empty;
        }
    }
}