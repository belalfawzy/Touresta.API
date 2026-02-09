using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByIdAsync(int id)
            => await _db.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email)
            => await _db.Users.SingleOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUserIdAsync(string userId)
            => await _db.Users.SingleOrDefaultAsync(u => u.UserId == userId);

        public bool ExistsByEmail(string email)
            => _db.Users.Any(u => u.Email.ToLower() == email.ToLower());

        public User? FindByEmail(string email)
            => _db.Users.SingleOrDefault(u => u.Email == email);

        public void Add(User user)
            => _db.Users.Add(user);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();

        public void SaveChanges()
            => _db.SaveChanges();
    }
}
