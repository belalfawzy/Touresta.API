using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _memoryCache;

    public AuthService(AppDbContext db, IConfiguration config, EmailService emailService, IMemoryCache memoryCache)
    {
        _db = db;
        _config = config;
        _userHasher = new PasswordHasher<User>();
        _adminHasher = new PasswordHasher<Admin>();
        _emailService = emailService;
        _memoryCache = memoryCache;
    }

    // ==================== USER METHODS ====================

    public (bool Success, string Message, User? User) CheckEmail(string email)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null) return (false, "This email doesn't exist", null);
        return (true, "Email exists", user);
    }

    public (bool Success, string Token, string Message, User? User) VerifyPassword(string email, string password)
    {
        try
        {
            var user = _db.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
                return (false, string.Empty, "User not found", null);

            if (string.IsNullOrEmpty(user.PasswordHash))
                return (false, string.Empty, "Password not set for this user", null);

            var res = _userHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (res == PasswordVerificationResult.Failed)
                return (false, string.Empty, "Invalid password", null);

            var token = GenerateUserJwtToken(user);

            if (string.IsNullOrEmpty(token))
                return (false, string.Empty, "Token generation failed", null);

            return (true, token, "Login successful", user);
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"Server error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, string? Code)> SendVerificationCode(string email)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null) return (false, "This email doesn't exist", null);

        var code = new Random().Next(100000, 999999).ToString();

        // حفظ الكود في الـ Memory Cache
        var cacheKey = $"VerificationCode_{email}";
        _memoryCache.Set(cacheKey, code, TimeSpan.FromMinutes(10));

        return (true, "Verification code sent to email", code);
    }

    public (bool Success, string Token, string Message, User? User) VerifyCode(string email, string code)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null) return (false, string.Empty, "This email doesn't exist", null);

        // التحقق من الكود من الـ Memory Cache
        var cacheKey = $"VerificationCode_{email}";
        if (!_memoryCache.TryGetValue(cacheKey, out string storedCode) || storedCode != code)
            return (false, string.Empty, "Invalid or expired code", null);

        user.IsVerified = true;
        _db.SaveChanges();

        // مسح الكود من الـ Cache بعد الاستخدام
        _memoryCache.Remove(cacheKey);

        var token = GenerateUserJwtToken(user);
        return (true, token, "Verification successful", user);
    }
    public async Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterRequest req)
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
            ProfileImageUrl = req.ProfileImageUrl,
            UserId = Guid.NewGuid().ToString(),  
            GoogleId = "",
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = ""
        };

        user.PasswordHash = _userHasher.HashPassword(user, req.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return (true, "User registered successfully", user);
    }

    public async Task<(bool Success, string Token, string Message, User? User)> RegisterWithGoogleAsync(GoogleRegisterRequest req)
    {
        if (_db.Users.Any(u => u.Email == req.Email))
            return (false, string.Empty, "This email is already registered", null);

        var user = new User
        {
            Email = req.Email,
            UserName = req.Name,
            IsVerified = true,
            PasswordHash = "",
            PhoneNumber = "",
            Gender = "",
            Country = "",
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateUserJwtToken(user);
        return (true, token, "Account created successfully with Google", user);
    }

    public async Task<(bool Success, string? Email, string Message, User? User)> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            // محاكاة للتحقق من توكن Google - استبدل هذا بالتنفيذ الحقيقي
            await Task.Delay(100);

            // في الواقع، هنا ستقوم بالتحقق من التوكن مع Google APIs
            var email = "user@gmail.com"; // سيكون من التحقق الفعلي

            var user = _db.Users.SingleOrDefault(u => u.Email == email);
            return (true, email, "Google account verified successfully", user);
        }
        catch (Exception ex)
        {
            return (false, null, $"Verification failed: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, bool UserExists, User? User, string? Code)> ProcessGoogleSignIn(string email, string googleId, string name)
    {
        try
        {
            var existingUser = _db.Users.SingleOrDefault(u => u.Email == email);

            // لو اليوزر موجود
            if (existingUser != null)
            {
                var code = new Random().Next(100000, 999999).ToString();

                // حفظ الكود في الـ Memory Cache
                var cacheKey = $"VerificationCode_{email}";
                _memoryCache.Set(cacheKey, code, TimeSpan.FromMinutes(10));

                return (true, "User exists - verification code sent", true, existingUser, code);
            }
            // لو اليوزر مش موجود
            else
            {
                return (true, "User not found - redirect to registration", false, null, null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", false, null, null);
        }
    }

    public string? GenerateUserJwtToken(User user)
    {
        try
        {
            var keyString = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutes = _config["Jwt:ExpireMinutes"];

            if (string.IsNullOrEmpty(keyString))
                throw new ArgumentException("JWT Key is missing in configuration");

            if (string.IsNullOrEmpty(issuer))
                throw new ArgumentException("JWT Issuer is missing in configuration");

            if (string.IsNullOrEmpty(audience))
                audience = issuer;

            if (!int.TryParse(expireMinutes, out int expireMinutesValue))
                expireMinutesValue = 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName ?? ""),
                new Claim("email", user.Email),
                new Claim("type", "user")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutesValue),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JWT Generation Error: {ex.Message}");
            return null;
        }
    }

    // ==================== ADMIN METHODS ====================

    public (bool Success, string Message, Admin? Admin) CheckAdminEmail(string email)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, "This email is not registered in our system.", null);

        return (true, "Email exists", admin);
    }

    public (bool Success, string Token, string Message, Admin? Admin) VerifyAdminPassword(string email, string password)
    {
        try
        {
            var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
            if (admin == null)
                return (false, string.Empty, "Admin not found", null);

            if (string.IsNullOrEmpty(admin.PasswordHash))
                return (false, string.Empty, "Password not set for this admin", null);

            var result = _adminHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                return (false, string.Empty, "Invalid password. Please try again.", null);

            var token = GenerateAdminJwtToken(admin);

            if (string.IsNullOrEmpty(token))
                return (false, string.Empty, "Token generation failed", null);

            return (true, token, "Login successful", admin);
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"Server error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, string? Otp)> SendAdminVerificationCode(string email)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, "This account is not allowed to sign in within this network. Please talk to your network administrator for more information.", null);

        var otp = new Random().Next(100000, 999999).ToString();

        // استخدام Memory Cache للمسؤولين
        var cacheKey = $"AdminVerificationCode_{email}";
        _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(10));

        var emailSent = await _emailService.SendOtpEmail(email, otp);

        if (emailSent)
        {
            return (true, "OTP sent to your email", otp);
        }
        else
        {
            return (true, "OTP generated", otp);
        }
    }

    public (bool Success, string Token, string Message, Admin? Admin) VerifyAdminOtp(string email, string otp)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, string.Empty, "Admin not found", null);

        // التحقق من الكود من الـ Memory Cache
        var cacheKey = $"AdminVerificationCode_{email}";
        if (!_memoryCache.TryGetValue(cacheKey, out string storedOtp) || storedOtp != otp)
            return (false, string.Empty, "Invalid or expired OTP. Please try again.", null);

        // مسح الكود من الـ Cache بعد الاستخدام
        _memoryCache.Remove(cacheKey);

        var token = GenerateAdminJwtToken(admin);
        if (string.IsNullOrEmpty(token))
            return (false, string.Empty, "Token generation failed", null);

        return (true, token, "Login successful", admin);
    }

    public string? GenerateAdminJwtToken(Admin admin)
    {
        try
        {
            var keyString = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutes = _config["Jwt:ExpireMinutes"];

            if (string.IsNullOrEmpty(keyString))
                throw new ArgumentException("JWT Key is missing in configuration");

            if (string.IsNullOrEmpty(issuer))
                throw new ArgumentException("JWT Issuer is missing in configuration");

            if (string.IsNullOrEmpty(audience))
                audience = issuer;

            if (!int.TryParse(expireMinutes, out int expireMinutesValue))
                expireMinutesValue = 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", admin.Id.ToString()),
                new Claim("role", admin.Role.ToString()),
                new Claim("fullName", admin.FullName ?? ""),
                new Claim("email", admin.Email),
                new Claim("type", "admin")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutesValue),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JWT Generation Error: {ex.Message}");
            return null;
        }
    }
}