using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface IHelperReportRepository
    {
        IQueryable<HelperReport> Query();
        Task<HelperReport?> GetByIdAsync(string id);
        void Add(HelperReport report);
        Task SaveChangesAsync();
    }
}