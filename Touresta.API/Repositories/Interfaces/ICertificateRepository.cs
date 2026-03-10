using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate?> GetByIdAndHelperIdAsync(string certificateId, string helperId);
        void Add(Certificate certificate);
        void Remove(Certificate certificate);
        Task SaveChangesAsync();
    }
}
