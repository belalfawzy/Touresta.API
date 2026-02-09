using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _db;

        public AdminRepository(AppDbContext db)
        {
            _db = db;
        }

        public Admin? FindActiveByEmail(string email)
            => _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();

        public void SaveChanges()
            => _db.SaveChanges();
    }
}
