using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AppDbContext _db;

        public CertificateRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Certificate?> GetByIdAndHelperIdAsync(int certificateId, int helperId)
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
