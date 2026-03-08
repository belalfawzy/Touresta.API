using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IAdminAuditLogRepository
    {
        void Add(AdminAuditLog log);
        Task SaveChangesAsync();

        IQueryable<AdminAuditLog> Query();
        Task<List<AdminAuditLog>> GetByTargetAsync(string targetType, int targetId);
    }
}