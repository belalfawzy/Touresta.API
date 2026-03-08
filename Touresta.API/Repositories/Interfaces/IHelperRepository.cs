using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IHelperRepository
    {
        Task<Helper?> GetByIdAsync(int id);
        Task<Helper?> GetByIdWithCertificatesAsync(int id);
        Task<Helper?> GetByUserIdAsync(int userId);
        Task<Helper?> GetByIdWithFullIncludesAsync(int id);
        Task<Helper?> GetByUserIdWithFullIncludesAsync(int userId);
        Task<Helper?> GetByIdWithDrugTestsAsync(int id);
        Task<Helper?> GetByIdWithCarAsync(int id);

        Task<bool> ExistsByUserIdAsync(int userId);

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