using RAFIQ.API.Data;
using RAFIQ.API.Models;
using RAFIQ.API.Repositories.Interfaces;

namespace RAFIQ.API.Repositories.Implementations
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
