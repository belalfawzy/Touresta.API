using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Admin? FindActiveByEmail(string email);
        Task<Admin?> GetByIdAsync(string id);
        Task<List<Admin>> GetAllAsync();
        Task<bool> ExistsByEmailAsync(string email);
        void Add(Admin admin);
        Task SaveChangesAsync();
        void SaveChanges();
    }
}