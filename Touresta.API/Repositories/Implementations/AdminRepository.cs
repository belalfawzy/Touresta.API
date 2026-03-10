using Microsoft.EntityFrameworkCore;
using RAFIQ.API.Data;
using RAFIQ.API.Models;
using RAFIQ.API.Repositories.Interfaces;

namespace RAFIQ.API.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _db;

        public AdminRepository(AppDbContext db)
        {
            _db = db;
        }

        public Admin? FindActiveByEmail(string email) =>
            _db.Admins.SingleOrDefault(a => a.Email == email && a.IsActive);

        public async Task<Admin?> GetByIdAsync(string id) =>
            await _db.Admins.FindAsync(id);

        public async Task<List<Admin>> GetAllAsync() =>
            await _db.Admins.OrderByDescending(a => a.CreatedAt).ToListAsync();

        public async Task<bool> ExistsByEmailAsync(string email) =>
            await _db.Admins.AnyAsync(a => a.Email == email);

        public void Add(Admin admin) => _db.Admins.Add(admin);

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

        public void SaveChanges() => _db.SaveChanges();
    }
}