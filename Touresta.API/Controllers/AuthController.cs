using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.DTOs;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// User authentication, registration, and profile management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("User Authentication")]
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

        /// <summary>
        /// Register a new user account.
        /// </summary>
        /// <remarks>
        /// Creates a new user and sends a 6-digit verification code to the provided email.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/register
        ///     {
        ///         "email": "john@example.com",
        ///         "userName": "JohnDoe",
        ///         "password": "SecurePass@123",
        ///         "phoneNumber": "+201234567890",
        ///         "gender": "Male",
        ///         "birthDate": "1995-06-15",
        ///         "country": "Egypt"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "Account created. Verification code sent to your email.",
        ///         "action": "enter_verification_code",
        ///         "user": {
        ///             "id": 1,
        ///             "userId": "a1b2c3d4-...",
        ///             "email": "john@example.com",
        ///             "userName": "JohnDoe",
        ///             "phoneNumber": "+201234567890",
        ///             "gender": "Male",
        ///             "country": "Egypt",
        ///             "birthDate": "1995-06-15",
        ///             "profileImageUrl": "/images/users/default.png",
        ///             "type": "user"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="req">User registration details.</param>
        /// <response code="200">Account created, verification code sent.</response>
        /// <response code="400">Email already registered or invalid data.</response>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var (success, message, userId) = await _auth.RegisterAsync(req);

            if (!success)
                return BadRequest(new { message });

            var user = await _db.Users
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return Ok(new { message, userId });

            if (string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                user.ProfileImageUrl = "/images/users/default.png";
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                message,
                action = "enter_verification_code",
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
                    profileImageUrl = user.ProfileImageUrl,
                    type = "user"
                }
            });
        }

        /// <summary>
        /// Check if a user email exists.
        /// </summary>
        /// <remarks>
        /// Verifies whether the provided email is registered in the system.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/check-email
        ///     {
        ///         "email": "john@example.com"
        ///     }
        ///
        /// **Success Response (email found):**
        ///
        ///     {
        ///         "message": "Email exists",
        ///         "action": "go_to_password_page",
        ///         "email": "john@example.com"
        ///     }
        ///
        /// **Not Found Response:**
        ///
        ///     {
        ///         "message": "This email doesn't exist",
        ///         "action": "stay_on_email_page"
        ///     }
        /// </remarks>
        /// <param name="req">The email to check.</param>
        /// <response code="200">Email exists.</response>
        /// <response code="404">Email not found.</response>
        [HttpPost("check-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult CheckEmail([FromBody] EmailRequest req)
        {
            var (success, message) = _auth.CheckEmail(req.Email);
            if (!success) return NotFound(new { message, action = "stay_on_email_page" });
            return Ok(new { message, action = "go_to_password_page", email = req.Email });
        }

        /// <summary>
        /// Login with email and password.
        /// </summary>
        /// <remarks>
        /// Verifies user credentials. Returns a JWT token if the account is verified.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/verify-password
        ///     {
        ///         "email": "john@example.com",
        ///         "password": "SecurePass@123"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIs...",
        ///         "message": "Login successful",
        ///         "action": "go_to_dashboard",
        ///         "user": {
        ///             "id": 1,
        ///             "userId": "a1b2c3d4-...",
        ///             "email": "john@example.com",
        ///             "userName": "JohnDoe",
        ///             "profileImageUrl": "/images/users/default.png"
        ///         }
        ///     }
        ///
        /// **Unverified Email Response:**
        ///
        ///     {
        ///         "message": "Please verify your email with the OTP first.",
        ///         "action": "email_not_verified",
        ///         "email": "john@example.com"
        ///     }
        /// </remarks>
        /// <param name="req">Login credentials.</param>
        /// <response code="200">Login successful, JWT token returned.</response>
        /// <response code="400">Invalid credentials or unverified email.</response>
        [HttpPost("verify-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult VerifyPassword([FromBody] LoginRequest req)
        {
            var (success, token, message) = _auth.VerifyPassword(req.Email, req.Password);

            if (!success)
            {
                return BadRequest(new
                {
                    message,
                    action = "email_not_verified",
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

        /// <summary>
        /// Google login - sends verification code to email.
        /// </summary>
        /// <remarks>
        /// For existing users who signed up with Google. Sends a 6-digit code to the user's email.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/google-login
        ///     {
        ///         "email": "john@gmail.com"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "Verification code sent to email"
        ///     }
        /// </remarks>
        /// <param name="req">User email for Google login.</param>
        /// <response code="200">Verification code sent.</response>
        /// <response code="404">Email not registered.</response>
        [HttpPost("google-login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GoogleLogin([FromBody] EmailRequest req)
        {
            var (success, message, code) = _auth.GoogleLogin(req.Email);
            if (!success) return NotFound(new { message });

            if (!string.IsNullOrEmpty(code))
            {
                await _emailService.SendOtpEmail(req.Email, code);
            }

            return Ok(new
            {
                message = "Verification code sent to email",
            });
        }

        /// <summary>
        /// Verify email verification code.
        /// </summary>
        /// <remarks>
        /// Validates the 6-digit code sent to the user's email. Returns a JWT token on success.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/verify-code
        ///     {
        ///         "email": "john@example.com",
        ///         "code": "123456"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIs...",
        ///         "message": "Verification successful"
        ///     }
        /// </remarks>
        /// <param name="req">Email and verification code.</param>
        /// <response code="200">Code verified, JWT token returned.</response>
        /// <response code="400">Invalid or expired code.</response>
        [HttpPost("verify-code")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult VerifyCode([FromBody] VerifyCodeRequest req)
        {
            var (success, token, message) = _auth.VerifyCode(req.Email, req.Code);
            if (!success) return BadRequest(new { message });
            return Ok(new { token, message });
        }

        /// <summary>
        /// Register or activate account via Google.
        /// </summary>
        /// <remarks>
        /// Creates a new account using Google credentials, or activates an existing unverified account.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/google-register
        ///     {
        ///         "email": "john@gmail.com",
        ///         "googleId": "google-uid-123456",
        ///         "name": "John Doe",
        ///         "profileImageUrl": "https://lh3.googleusercontent.com/..."
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIs...",
        ///         "message": "Account created successfully with Google",
        ///         "action": "go_to_dashboard",
        ///         "user": {
        ///             "id": 1,
        ///             "userId": "a1b2c3d4-...",
        ///             "email": "john@gmail.com",
        ///             "userName": "John Doe",
        ///             "type": "user"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="req">Google registration details.</param>
        /// <response code="200">Account created or activated.</response>
        /// <response code="400">Registration failed.</response>
        [HttpPost("google-register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RegisterWithGoogle([FromBody] GoogleRegisterRequest req)
        {
            var (success, token, message) = await _auth.RegisterWithGoogleAsync(req);

            if (!success)
                return BadRequest(new { message });

            var user = _db.Users.SingleOrDefault(u => u.Email == req.Email);
            if (user == null)
                return Ok(new { token, message });

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
                    profileImageUrl = user.ProfileImageUrl,
                    type = "user"
                }
            });
        }

        /// <summary>
        /// Verify Google login code.
        /// </summary>
        /// <remarks>
        /// Validates the verification code sent during the Google login flow. Returns a JWT token on success.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/google-verify-code
        ///     {
        ///         "email": "john@gmail.com",
        ///         "code": "654321"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIs...",
        ///         "message": "Google account verified successfully",
        ///         "action": "go_to_dashboard",
        ///         "user": {
        ///             "id": 1,
        ///             "userId": "a1b2c3d4-...",
        ///             "email": "john@gmail.com",
        ///             "userName": "John Doe",
        ///             "type": "user"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="req">Email and verification code.</param>
        /// <response code="200">Code verified, JWT token and user data returned.</response>
        /// <response code="400">Invalid or expired code.</response>
        [HttpPost("google-verify-code")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult VerifyGoogleCode([FromBody] VerifyCodeRequest req)
        {
            var (success, token, message) = _auth.VerifyGoogleCode(req.Email, req.Code);

            if (!success)
                return BadRequest(new { message });

            var user = _db.Users.SingleOrDefault(u => u.Email == req.Email);
            if (user == null)
                return Ok(new { token, message });

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
                    profileImageUrl = user.ProfileImageUrl,
                    type = "user"
                }
            });
        }

        /// <summary>
        /// Verify Google ID token and send verification code.
        /// </summary>
        /// <remarks>
        /// Verifies a Google ID token and sends an OTP code to the associated email.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/verify-google-token
        ///     {
        ///         "idToken": "eyJhbGciOiJSUzI1NiIs..."
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "Verification code sent to email",
        ///         "action": "enter_verification_code",
        ///         "email": "john@gmail.com"
        ///     }
        ///
        /// **Not Registered Response:**
        ///
        ///     {
        ///         "message": "Google account not registered",
        ///         "action": "redirect_to_signup",
        ///         "email": "john@gmail.com"
        ///     }
        /// </remarks>
        /// <param name="req">Google ID token.</param>
        /// <response code="200">Token verified, verification code sent.</response>
        /// <response code="400">Invalid token.</response>
        /// <response code="404">Google account not registered.</response>
        [HttpPost("verify-google-token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Request a password reset code.
        /// </summary>
        /// <remarks>
        /// Sends a 6-digit reset code to the user's email. The code expires in 10 minutes.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/forgot-password
        ///     {
        ///         "email": "john@example.com"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "Reset code sent to email"
        ///     }
        /// </remarks>
        /// <param name="req">User email.</param>
        /// <response code="200">Reset code sent.</response>
        /// <response code="400">Email is missing.</response>
        /// <response code="404">Email not found.</response>
        [HttpPost("forgot-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailRequest req)
        {
            if (string.IsNullOrEmpty(req.Email))
                return BadRequest(new { message = "Email is required" });

            var (success, message) = await _auth.SendForgotPasswordCodeAsync(req.Email);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }

        /// <summary>
        /// Reset password with verification code.
        /// </summary>
        /// <remarks>
        /// Resets the user's password using the code received via email.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/reset-password
        ///     {
        ///         "email": "john@example.com",
        ///         "code": "123456",
        ///         "newPassword": "NewSecurePass@456"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "Password reset successfully"
        ///     }
        /// </remarks>
        /// <param name="req">Email, code, and new password.</param>
        /// <response code="200">Password reset successful.</response>
        /// <response code="400">Invalid code or missing fields.</response>
        [HttpPost("reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
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

        /// <summary>
        /// Update user profile.
        /// </summary>
        /// <remarks>
        /// Updates user profile fields and optionally uploads a new profile image.
        /// Send as **multipart/form-data**. Only provided fields will be updated.
        ///
        /// **Form Fields:**
        /// - `UserId` (required) - The user's unique ID
        /// - `UserName` (optional) - New display name
        /// - `PhoneNumber` (optional) - New phone number
        /// - `Country` (optional) - New country
        /// - `Gender` (optional) - New gender
        /// - `BirthDate` (optional) - New birth date
        /// - `profileImage` (optional, file) - New profile image
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "success": true,
        ///         "message": "Profile updated successfully",
        ///         "user": {
        ///             "userId": "a1b2c3d4-...",
        ///             "email": "john@example.com",
        ///             "userName": "JohnUpdated",
        ///             "phoneNumber": "+201234567890",
        ///             "country": "Egypt",
        ///             "gender": "Male",
        ///             "birthDate": "1995-06-15",
        ///             "profileImage": "/images/users/abc123.jpg"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="request">Profile fields to update.</param>
        /// <param name="profileImage">Optional profile image file.</param>
        /// <response code="200">Profile updated.</response>
        /// <response code="400">UserId is missing.</response>
        /// <response code="404">User not found.</response>
        [HttpPut("update-profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Consumes("multipart/form-data")]
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

        /// <summary>
        /// Resend verification code to email.
        /// </summary>
        /// <remarks>
        /// Generates a new 6-digit verification code and sends it to the user's email.
        /// Only works for unverified accounts.
        ///
        /// **Example Request:**
        ///
        ///     POST /api/auth/resend-verification-code
        ///     {
        ///         "email": "john@example.com"
        ///     }
        ///
        /// **Success Response:**
        ///
        ///     {
        ///         "message": "Verification code sent again",
        ///         "action": "enter_verification_code",
        ///         "email": "john@example.com"
        ///     }
        /// </remarks>
        /// <param name="req">User email.</param>
        /// <response code="200">New verification code sent.</response>
        /// <response code="400">User not found or already verified.</response>
        [HttpPost("resend-verification-code")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
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
    }
}
