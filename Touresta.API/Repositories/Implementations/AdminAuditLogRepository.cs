using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class AdminAuditLogRepository : IAdminAuditLogRepository
    {
        private readonly AppDbContext _db;

        public AdminAuditLogRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Add(AdminAuditLog log) => _db.AdminAuditLogs.Add(log);

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

        public IQueryable<AdminAuditLog> Query() => _db.AdminAuditLogs.AsNoTracking();

        public async Task<List<AdminAuditLog>> GetByTargetAsync(string targetType, string targetId)
        {
            return await _db.AdminAuditLogs
                .AsNoTracking()
                .Where(x => x.TargetType == targetType && x.TargetId == targetId)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }
    }
}