using Microsoft.EntityFrameworkCore;
using RAFIQ.API.Data;
using RAFIQ.API.Models;
using RAFIQ.API.Repositories.Interfaces;

namespace RAFIQ.API.Repositories.Implementations
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AppDbContext _db;

        public CertificateRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Certificate?> GetByIdAndHelperIdAsync(string certificateId, string helperId)
            => await _db.Certificates.FirstOrDefaultAsync(
                c => c.Id == certificateId && c.HelperId == helperId);

        public void Add(Certificate certificate)
            => _db.Certificates.Add(certificate);

        public void Remove(Certificate certificate)
            => _db.Certificates.Remove(certificate);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
