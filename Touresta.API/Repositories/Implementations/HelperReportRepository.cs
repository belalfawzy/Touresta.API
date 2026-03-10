using Microsoft.EntityFrameworkCore;
using RAFIQ.API.Data;
using RAFIQ.API.Models;
using RAFIQ.API.Repositories.Interfaces;

namespace RAFIQ.API.Repositories.Implementations
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

        public async Task<HelperReport?> GetByIdAsync(string id) =>
            await _db.HelperReports.FirstOrDefaultAsync(r => r.Id == id);

        public void Add(HelperReport report) => _db.HelperReports.Add(report);

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}