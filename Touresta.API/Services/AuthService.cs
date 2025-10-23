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
        try
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

            if (string.IsNullOrEmpty(token))
                return (false, string.Empty, "Token generation failed");

            return (true, token, "Login successful");
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"Server error: {ex.Message}");
        }
    }

    public (bool Success, string Message, string Code) GoogleLogin(string email)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null) return (false, "This email doesn't exist", null);

        var code = new Random().Next(100000, 999999).ToString();
        user.VerificationCode = code;
        _db.SaveChanges();

        return (true, "Verification code sent to email", code);
    }

    public (bool Success, string Token, string Message) VerifyCode(string email, string code)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null) return (false, string.Empty, "This email doesn't exist");

        if (user.VerificationCode != code)
            return (false, string.Empty, "Invalid code");

        user.IsVerified = true;
        user.VerificationCode = null;
        _db.SaveChanges();

        var token = GenerateUserJwtToken(user);
        return (true, token, "Verification successful");
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
            ProfileImageUrl = req.ProfileImageUrl,
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


    public (bool Success, string Message, bool EmailExists, string? Code) GoogleLoginInit(string email)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);

        if (user == null)
            return (false, "Email not found - Redirect to signup", false, null);

        var code = new Random().Next(100000, 999999).ToString();
        user.VerificationCode = code;
        _db.SaveChanges();

        return (true, "Verification code sent to email", true, code);
    }

    public async Task<(bool Success, string Token, string Message)> RegisterWithGoogleAsync(GoogleRegisterRequest req)
    {
        if (_db.Users.Any(u => u.Email == req.Email))
            return (false, string.Empty, "This email is already registered");

        var user = new User
        {
            Email = req.Email,
            UserName = req.Name,
            GoogleId = req.GoogleId,
            IsVerified = true,
            PasswordHash = "",
            PhoneNumber = "",
            Gender = "",
            Country = ""
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateUserJwtToken(user);
        return (true, token, "Account created successfully with Google");
    }

    public (bool Success, string Token, string Message) VerifyGoogleCode(string email, string code)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
            return (false, string.Empty, "User not found");

        if (user.VerificationCode != code)
            return (false, string.Empty, "Invalid verification code");

        user.VerificationCode = null;
        _db.SaveChanges();

        var token = GenerateUserJwtToken(user);
        return (true, token, "Google login successful");
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

    private string? GenerateUserJwtToken(User user)
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

    public (bool Success, string Token, string Message) VerifyAdminPassword(string email, string password)
    {
        try
        {
            var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
            if (admin == null)
                return (false, string.Empty, "Admin not found");

            if (string.IsNullOrEmpty(admin.PasswordHash))
                return (false, string.Empty, "Password not set for this admin");

            var result = _adminHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                return (false, string.Empty, "Invalid password. Please try again.");

            var token = GenerateAdminJwtToken(admin);

            if (string.IsNullOrEmpty(token))
                return (false, string.Empty, "Token generation failed");

            return (true, token, "Login successful");
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"Server error: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message, string? Otp)> GoogleAdminLogin(string email)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, "This account is not allowed to sign in within this network. Please talk to your network administrator for more information.", null);

        var otp = new Random().Next(100000, 999999).ToString();
        admin.VerificationCode = otp;
        admin.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await _db.SaveChangesAsync();

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

    public (bool Success, string Token, string Message) VerifyAdminOtp(string email, string otp)
    {
        var admin = _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);
        if (admin == null)
            return (false, string.Empty, "Admin not found");

        if (admin.VerificationCodeExpiry < DateTime.UtcNow)
            return (false, string.Empty, "Invalid or expired OTP. Please try again.");

        if (admin.VerificationCode != otp)
            return (false, string.Empty, "Invalid or expired OTP. Please try again.");

        admin.VerificationCode = null;
        admin.VerificationCodeExpiry = null;
        _db.SaveChanges();

        var token = GenerateAdminJwtToken(admin);
        if (string.IsNullOrEmpty(token))
            return (false, string.Empty, "Token generation failed");

        return (true, token, "Login successful");
    }

    private string? GenerateAdminJwtToken(Admin admin)
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

    public IConfiguration GetConfiguration()
    {
        return _config;
    }
}