using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;

namespace Touresta.API.Services
{
    public class AutoCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AutoCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(6);

        public AutoCleanupService(IServiceScopeFactory scopeFactory, ILogger<AutoCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AutoCleanupService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var now = DateTime.UtcNow;

                        // Remove unverified users older than 24 hours
                        var unverifiedUsers = await db.Users
                            .Where(u => !u.IsVerified && u.CreatedAt < now.AddHours(-24))
                            .ToListAsync(stoppingToken);

                        if (unverifiedUsers.Any())
                        {
                            db.Users.RemoveRange(unverifiedUsers);
                            _logger.LogInformation("Removed {Count} unverified users", unverifiedUsers.Count);
                        }

                        // Clear expired verification codes
                        var expiredCodes = await db.Users
                            .Where(u => u.VerificationCodeExpiry.HasValue && u.VerificationCodeExpiry < now)
                            .ToListAsync(stoppingToken);

                        foreach (var user in expiredCodes)
                        {
                            user.VerificationCode = null;
                            user.VerificationCodeExpiry = null;
                        }

                        if (expiredCodes.Any())
                            _logger.LogInformation("Cleared expired codes for {Count} users", expiredCodes.Count);

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in AutoCleanupService");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
