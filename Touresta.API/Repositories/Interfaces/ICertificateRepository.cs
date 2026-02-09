using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate?> GetByIdAndHelperIdAsync(int certificateId, int helperId);
        void Add(Certificate certificate);
        void Remove(Certificate certificate);
        Task SaveChangesAsync();
    }
}
