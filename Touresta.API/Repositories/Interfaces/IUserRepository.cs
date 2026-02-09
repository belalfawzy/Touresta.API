using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserIdAsync(string userId);
        bool ExistsByEmail(string email);
        User? FindByEmail(string email);
        void Add(User user);
        Task SaveChangesAsync();
        void SaveChanges();
    }
}
