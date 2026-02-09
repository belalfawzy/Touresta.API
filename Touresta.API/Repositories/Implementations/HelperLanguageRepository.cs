using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class HelperLanguageRepository : IHelperLanguageRepository
    {
        private readonly AppDbContext _db;

        public HelperLanguageRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<HelperLanguage?> GetByHelperAndCodeAsync(int helperId, string languageCode)
            => await _db.HelperLanguages
                .Include(hl => hl.TestHistory)
                .FirstOrDefaultAsync(hl => hl.HelperId == helperId && hl.LanguageCode == languageCode);

        public async Task<List<string>> GetLanguageCodesByHelperIdAsync(int helperId)
            => await _db.HelperLanguages
                .Where(hl => hl.HelperId == helperId)
                .Select(hl => hl.LanguageCode)
                .ToListAsync();

        public async Task<List<HelperLanguage>> GetByHelperIdAsync(int helperId)
            => await _db.HelperLanguages
                .Where(hl => hl.HelperId == helperId)
                .ToListAsync();

        public void Add(HelperLanguage helperLanguage)
            => _db.HelperLanguages.Add(helperLanguage);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
