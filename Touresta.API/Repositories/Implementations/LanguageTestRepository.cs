using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class LanguageTestRepository : ILanguageTestRepository
    {
        private readonly AppDbContext _db;

        public LanguageTestRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Add(LanguageTest languageTest)
            => _db.LanguageTests.Add(languageTest);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
