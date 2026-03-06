using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IHelperReportRepository
    {
        IQueryable<HelperReport> Query();
        Task<HelperReport?> GetByIdAsync(int id);
        void Add(HelperReport report);
        Task SaveChangesAsync();
    }
}