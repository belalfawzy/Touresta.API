using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.DTOs;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _userHasher;
    private readonly PasswordHasher<Admin> _adminHasher;
    private readonly EmailService _emailService;

    public AuthService(AppDbContext db, IConfiguration config, EmailService emailService)
    {
        _db = db;
        _config = config;
        _userHasher = new PasswordHasher<User>();
        _adminHasher = new PasswordHasher<Admin>();
        _emailService = emailService;
    }

    // ==================== USER METHODS ====================

    public (bool Success, string Message) CheckEmail(string email)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null) return (false, "This email doesn't exist");
        return (true, "Email exists");
    }

    public (bool Success, string Token, string Message) VerifyPassword(string email, string password)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
            return (false, string.Empty, "User not found");

        if (string.IsNullOrEmpty(user.PasswordHash))
            return (false, string.Empty, "Password not set for this user");

        var res = _userHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (res == PasswordVerificationResult.Failed)
            return (false, string.Empty, "Invalid password");

        var token = GenerateUserJwtToken(user);
        return (true, token, "Login successful");
    }

    public async Task<(bool Success, string Message, string? UserId)> RegisterAsync(RegisterRequest req)
    {
        var exists = _db.Users.Any(u => u.Email.ToLower() == req.Email.ToLower());
        if (exists)
            return (false, "This email is already registered", null);

        var user = new User
        {
            Email = req.Email,
            UserName = req.UserName,
            PhoneNumber = req.PhoneNumber,
            Gender = req.Gender,
            BirthDate = req.BirthDate,
            Country = req.Country,
            ProfileImageUrl = string.IsNullOrEmpty(req.ProfileImageUrl) ? "/images/users/default.png" : req.ProfileImageUrl,
            UserId = Guid.NewGuid().ToString(),
            GoogleId = "",
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _userHasher.HashPassword(user, req.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return (true, "User registered successfully", user.UserId);
    }

    // ==================== GOOGLE ACCOUNT FLOW ====================

    // الخطوة 1: إرسال كود الجيميل
    public async Task<(bool Success, string Message, bool EmailExists, string? Code)> GoogleLoginInitAsync(string email, string googleId)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);

        var code = new Random().Next(100000, 999999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(10);

        if (user == null)
        {
            user = new User
            {
                Email = email,
                UserName = email.Split('@')[0],
                GoogleId = googleId,
                PasswordHash = "",
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                VerificationCode = code,
                VerificationCodeExpiry = expiry,
                ProfileImageUrl = "/images/users/default.png",
                UserId = Guid.NewGuid().ToString(),
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            await _emailService.SendOtpEmail(email, code);

            return (true, "Verification code sent to email (new Google user created)", false, code);
        }
        else
        {
            user.GoogleId = googleId;
            user.VerificationCode = code;
            user.VerificationCodeExpiry = expiry;
            await _db.SaveChangesAsync();
            await _emailService.SendOtpEmail(email, code);

            return (true, "Verification code re-sent to existing user", true, code);
        }
    }

    // الخطوة 2: التحقق من الكود يا ملك
    public (bool Success, string Token, string Message) VerifyGoogleCode(string email, string code)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
            return (false, string.Empty, "User not found");

        if (user.VerificationCodeExpiry.HasValue && user.VerificationCodeExpiry < DateTime.UtcNow)
            return (false, string.Empty, "Verification code expired");

        if (user.VerificationCode != code)
            return (false, string.Empty, "Invalid verification code");

        user.IsVerified = true;
        user.VerificationCode = null;
        user.VerificationCodeExpiry = null;

        _db.SaveChanges();

        var token = GenerateUserJwtToken(user);
        return (true, token, "Google account verified successfully");
    }

    // الخطوة 3: تسجيل جديد أو تفعيل ها قولي بسرعه 
    public async Task<(bool Success, string Token, string Message)> RegisterWithGoogleAsync(GoogleRegisterRequest req)
    {
        var existingUser = _db.Users.SingleOrDefault(u => u.Email == req.Email);

        if (existingUser != null)
        {
            if (existingUser.IsVerified)
            {
                var jwt = GenerateUserJwtToken(existingUser) ?? string.Empty;
                return (true, jwt, "User already registered and verified");
            }

            existingUser.GoogleId = req.GoogleId;
            existingUser.UserName = req.Name ?? existingUser.UserName;
            existingUser.ProfileImageUrl = string.IsNullOrWhiteSpace(req.ProfileImageUrl)
                ? "/images/users/default.png"
                : req.ProfileImageUrl!;
            existingUser.IsVerified = true;
            existingUser.VerificationCode = null;

            await _db.SaveChangesAsync();

            var jwt2 = GenerateUserJwtToken(existingUser) ?? string.Empty;
            return (true, jwt2, "Account verified and activated successfully");
        }

        var user = new User
        {
            Email = req.Email,
            UserName = req.Name ?? string.Empty,
            GoogleId = req.GoogleId,
            IsVerified = true,
            PasswordHash = string.Empty,
            PhoneNumber = string.Empty,
            Gender = string.Empty,
            Country = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid().ToString(),
            ProfileImageUrl = string.IsNullOrWhiteSpace(req.ProfileImageUrl)
                ? "/images/users/default.png"
                : req.ProfileImageUrl!
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var jwtNew = GenerateUserJwtToken(user) ?? string.Empty;
        return (true, jwtNew, "Account created successfully with Google");
    }

    // ==================== ADMIN METHODS ====================

    public (bool Success, string Message, Admin? Admin) CheckAdminEmail(string email)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, "This email is not registered in our system.", null);

        return (true, "Email exists", admin);
    }

    public (bool Success, string Token, string Message) VerifyAdminPassword(string email, string password)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, string.Empty, "Admin not found");

        var result = _adminHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return (false, string.Empty, "Invalid password");

        var token = GenerateAdminJwtToken(admin);
        return (true, token, "Login successful");
    }

    public async Task<(bool Success, string Message, string? Otp)> GoogleAdminLogin(string email)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, "Not authorized admin email.", null);

        var otp = new Random().Next(100000, 999999).ToString();
        admin.VerificationCode = otp;
        admin.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await _db.SaveChangesAsync();

        await _emailService.SendOtpEmail(email, otp);
        return (true, "OTP sent to admin email", otp);
    }

    public (bool Success, string Token, string Message) VerifyAdminOtp(string email, string otp)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, string.Empty, "Admin not found");

        if (admin.VerificationCodeExpiry < DateTime.UtcNow)
            return (false, string.Empty, "Invalid or expired OTP.");

        if (admin.VerificationCode != otp)
            return (false, string.Empty, "Invalid OTP.");

        admin.VerificationCode = null;
        admin.VerificationCodeExpiry = null;
        _db.SaveChanges();

        var token = GenerateAdminJwtToken(admin);
        return (true, token, "Admin verified successfully");
    }
    // ============== BACKWARD COMPATIBILITY METHODS ==============

    public (bool Success, string Message, string Code) GoogleLogin(string email)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null) return (false, "This email doesn't exist", null);

        var code = new Random().Next(100000, 999999).ToString();
        user.VerificationCode = code;
        user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        _db.SaveChanges();

        _emailService.SendOtpEmail(email, code);
        return (true, "Verification code sent to email", code);
    }

    public (bool Success, string Token, string Message) VerifyCode(string email, string code)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
            return (false, string.Empty, "This email doesn't exist");

        if (user.VerificationCode != code)
            return (false, string.Empty, "Invalid code");

        user.IsVerified = true;
        user.VerificationCode = null;
        user.VerificationCodeExpiry = null;
        _db.SaveChanges();

        var token = GenerateUserJwtToken(user);
        return (true, token, "Verification successful");
    }

    public async Task<(bool Success, string Email, string Message)> VerifyGoogleAccountAsync(string idToken)
    {
        try
        {
            await Task.Delay(100);
            
            return (true, "user@gmail.com", "Google account verified successfully");
        }
        catch (Exception ex)
        {
            return (false, null, $"Verification failed: {ex.Message}");
        }
    }

    // ==================== JWT METHODS ====================

    private string? GenerateUserJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", user.Id.ToString()),
            new Claim("username", user.UserName ?? ""),
            new Claim("type", "user")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:ExpireMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string? GenerateAdminJwtToken(Admin admin)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", admin.Id.ToString()),
            new Claim("role", admin.Role.ToString()),
            new Claim("type", "admin")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:ExpireMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
