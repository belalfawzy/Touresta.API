using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface IAdminAuditLogRepository
    {
        void Add(AdminAuditLog log);
        Task SaveChangesAsync();

        IQueryable<AdminAuditLog> Query();
        Task<List<AdminAuditLog>> GetByTargetAsync(string targetType, string targetId);
    }
}