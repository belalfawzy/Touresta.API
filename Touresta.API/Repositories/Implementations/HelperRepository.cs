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

        public async Task<Helper?> GetByIdAsync(int id) =>
            await _db.Helpers.FindAsync(id);

        public async Task<Helper?> GetByIdWithCertificatesAsync(int id) =>
            await _db.Helpers
                .Include(h => h.Certificates)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<Helper?> GetByUserIdAsync(int userId) =>
            await _db.Helpers.FirstOrDefaultAsync(h => h.UserId == userId);

        public async Task<Helper?> GetByIdWithFullIncludesAsync(int id) =>
            await _db.Helpers
                .Include(h => h.User)
                .Include(h => h.Car)
                .Include(h => h.Languages).ThenInclude(l => l.TestHistory)
                .Include(h => h.Certificates)
                .Include(h => h.DrugTests)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<Helper?> GetByUserIdWithFullIncludesAsync(int userId) =>
            await _db.Helpers
                .Include(h => h.User)
                .Include(h => h.Car)
                .Include(h => h.Languages).ThenInclude(l => l.TestHistory)
                .Include(h => h.Certificates)
                .Include(h => h.DrugTests)
                .FirstOrDefaultAsync(h => h.UserId == userId);

        public async Task<Helper?> GetByIdWithDrugTestsAsync(int id) =>
            await _db.Helpers
                .Include(h => h.DrugTests)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<Helper?> GetByIdWithCarAsync(int id) =>
            await _db.Helpers
                .Include(h => h.Car)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task<bool> ExistsByUserIdAsync(int userId) =>
            await _db.Helpers.AnyAsync(h => h.UserId == userId);

        public async Task<List<Helper>> GetPendingHelpersAsync() =>
            await _db.Helpers
                .Include(h => h.User)
                .Include(h => h.Languages)
                .Include(h => h.DrugTests)
                .Where(h => h.ApprovalStatus == ApprovalStatus.Pending || h.ApprovalStatus == ApprovalStatus.UnderReview)
                .OrderBy(h => h.CreatedAt)
                .ToListAsync();

        public async Task<(List<Helper> Items, int TotalCount)> GetHelpersForAdminPagedAsync(
      string? search,
      string? approvalStatus,
      bool? isApproved,
      bool? isActive,
      bool? isBanned,
      bool? isSuspended,
      int page,
      int pageSize)
        {
            var query = _db.Helpers
                .Include(h => h.User)
                .Include(h => h.Languages)
                .Include(h => h.DrugTests)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(h =>
                    h.FullName.ToLower().Contains(search) ||
                    h.HelperId.ToLower().Contains(search) ||
                    h.User.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(approvalStatus) &&
                Enum.TryParse<ApprovalStatus>(approvalStatus, true, out var parsedStatus))
            {
                query = query.Where(h => h.ApprovalStatus == parsedStatus);
            }

            if (isApproved.HasValue)
                query = query.Where(h => h.IsApproved == isApproved.Value);

            if (isActive.HasValue)
                query = query.Where(h => h.IsActive == isActive.Value);

            if (isBanned.HasValue)
                query = query.Where(h => h.IsBanned == isBanned.Value);

            if (isSuspended.HasValue)
                query = query.Where(h => h.IsSuspended == isSuspended.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(h => h.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<int> CountAllAsync()
        {
            return await _db.Helpers.CountAsync();
        }

        public async Task<int> CountPendingAsync()
        {
            return await _db.Helpers
                .CountAsync(h => h.ApprovalStatus == ApprovalStatus.Pending);
        }

        public async Task<int> CountApprovedAsync()
        {
            return await _db.Helpers
                .CountAsync(h => h.ApprovalStatus == ApprovalStatus.Approved);
        }

        public async Task<int> CountActiveAsync()
        {
            return await _db.Helpers
                .CountAsync(h => h.IsActive);
        }

        public async Task<int> CountSuspendedAsync()
        {
            return await _db.Helpers
                .CountAsync(h => h.IsSuspended);
        }

        public async Task<int> CountBannedAsync()
        {
            return await _db.Helpers
                .CountAsync(h => h.IsBanned);
        }

        public async Task<List<Helper>> GetRecentHelpersAsync(int count)
        {
            return await _db.Helpers
                .Include(h => h.User)
                .OrderByDescending(h => h.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        public async Task<List<Helper>> GetActiveHelpersWithExpiredDrugTestsAsync(DateTime now) =>
            await _db.Helpers
                .Where(h => h.IsActive)
                .Where(h => !h.IsBanned && !h.IsSuspended)
                .Where(h => !_db.DrugTests.Any(dt => dt.HelperId == h.Id && dt.IsCurrent && dt.ExpiryDate > now))
                .ToListAsync();

        public void Add(Helper helper) => _db.Helpers.Add(helper);

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}