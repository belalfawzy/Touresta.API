using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface ILanguageTestRepository
    {
        void Add(LanguageTest languageTest);
        Task SaveChangesAsync();
    }
}
