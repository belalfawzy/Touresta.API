using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class HelperReportRepository : IHelperReportRepository
    {
        private readonly AppDbContext _db;

        public HelperReportRepository(AppDbContext db)
        {
            _db = db;
        }

        public IQueryable<HelperReport> Query() =>
            _db.HelperReports
                .Include(r => r.Helper)
                .Include(r => r.User)
                .AsQueryable();

        public async Task<HelperReport?> GetByIdAsync(int id) =>
            await _db.HelperReports.FirstOrDefaultAsync(r => r.Id == id);

        public void Add(HelperReport report) => _db.HelperReports.Add(report);

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}