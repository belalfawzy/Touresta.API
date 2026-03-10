using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface IHelperRepository
    {
        Task<Helper?> GetByIdAsync(string id);
        Task<Helper?> GetByIdWithCertificatesAsync(string id);
        Task<Helper?> GetByUserIdAsync(string userId);
        Task<Helper?> GetByIdWithFullIncludesAsync(string id);
        Task<Helper?> GetByUserIdWithFullIncludesAsync(string userId);
        Task<Helper?> GetByIdWithDrugTestsAsync(string id);
        Task<Helper?> GetByIdWithCarAsync(string id);

        Task<bool> ExistsByUserIdAsync(string userId);

        Task<List<Helper>> GetPendingHelpersAsync();

        Task<(List<Helper> Items, int TotalCount)> GetHelpersForAdminPagedAsync(
            string? search,
            string? approvalStatus,
            bool? isApproved,
            bool? isActive,
            bool? isBanned,
            bool? isSuspended,
            int page,
            int pageSize);

        Task<List<Helper>> GetActiveHelpersWithExpiredDrugTestsAsync(DateTime now);
        Task<int> CountAllAsync();
        Task<int> CountPendingAsync();
        Task<int> CountApprovedAsync();
        Task<int> CountActiveAsync();
        Task<int> CountSuspendedAsync();
        Task<int> CountBannedAsync();

        Task<List<Helper>> GetRecentHelpersAsync(int count);

        void Add(Helper helper);
        Task SaveChangesAsync();
    }
}