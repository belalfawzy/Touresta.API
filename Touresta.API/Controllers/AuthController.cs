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

           
            var user = await _db.Users.FindAsync(userId);
            if (user != null && string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                
                user.ProfileImageUrl = "/images/users/default.png";
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                message,
                userId,
                profileImage = user?.ProfileImageUrl ?? "/images/users/default.png"
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
                return BadRequest(new { message, action = "stay_on_password_page" });

            return Ok(new
            {
                token,
                message,
                action = "go_to_dashboard",
                user = new { email = req.Email, type = "user" }
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
        //حفظ الصوره يا انس اهو 
        [HttpPost("upload-profile-image")]
        [ProducesResponseType(typeof(UploadProfileImageResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile imageFile, [FromForm] string userId)
        {
            if (imageFile == null || string.IsNullOrEmpty(userId))
                return BadRequest(new { success = false, message = "Missing userId or imageFile" });

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