using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.Enums;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class HelperRepository : IHelperRepository
    {
        private readonly AppDbContext _db;

        public HelperRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Helper?> GetByIdAsync(int id)
            => await _db.Helpers.FindAsync(id);

        public async Task<Helper?> GetByIdWithCertificatesAsync(int id)
            => await _db.Helpers
                .Include(h => h.Certificates)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<Helper?> GetByUserIdAsync(int userId)
            => await _db.Helpers.FirstOrDefaultAsync(h => h.UserId == userId);

        public async Task<Helper?> GetByIdWithFullIncludesAsync(int id)
            => await _db.Helpers
                .Include(h => h.User)
                .Include(h => h.Car)
                .Include(h => h.Languages).ThenInclude(l => l.TestHistory)
                .Include(h => h.Certificates)
                .Include(h => h.DrugTests)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<Helper?> GetByUserIdWithFullIncludesAsync(int userId)
            => await _db.Helpers
                .Include(h => h.User)
                .Include(h => h.Car)
                .Include(h => h.Languages).ThenInclude(l => l.TestHistory)
                .Include(h => h.Certificates)
                .Include(h => h.DrugTests)
                .FirstOrDefaultAsync(h => h.UserId == userId);

        public async Task<Helper?> GetByIdWithDrugTestsAsync(int id)
            => await _db.Helpers
                .Include(h => h.DrugTests)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<Helper?> GetByIdWithCarAsync(int id)
            => await _db.Helpers
                .Include(h => h.Car)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<bool> ExistsByUserIdAsync(int userId)
            => await _db.Helpers.AnyAsync(h => h.UserId == userId);

        public async Task<List<Helper>> GetPendingHelpersAsync()
            => await _db.Helpers
                .Include(h => h.User)
                .Include(h => h.Languages)
                .Include(h => h.DrugTests)
                .Where(h => h.ApprovalStatus == ApprovalStatus.Pending
                         || h.ApprovalStatus == ApprovalStatus.UnderReview)
                .OrderBy(h => h.CreatedAt)
                .ToListAsync();

        public async Task<List<Helper>> GetActiveHelpersWithExpiredDrugTestsAsync(DateTime now)
            => await _db.Helpers
                .Where(h => h.IsActive)
                .Where(h => !_db.DrugTests.Any(dt => dt.HelperId == h.Id && dt.IsCurrent && dt.ExpiryDate > now))
                .ToListAsync();

        public void Add(Helper helper)
            => _db.Helpers.Add(helper);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
