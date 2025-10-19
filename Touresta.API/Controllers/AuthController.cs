using Microsoft.AspNetCore.Mvc;
using Touresta.API.Data;
using Touresta.API.DTOs;

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
            var (success, message) = await _auth.RegisterAsync(req);
            return success ? Ok(new { message }) : BadRequest(new { message });
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
    }

    public class GoogleTokenRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }
}