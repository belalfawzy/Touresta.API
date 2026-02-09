using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Admin? FindActiveByEmail(string email);
        Task SaveChangesAsync();
        void SaveChanges();
    }
}
