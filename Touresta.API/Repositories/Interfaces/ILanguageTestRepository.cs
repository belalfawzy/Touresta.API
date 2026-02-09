using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface ILanguageTestRepository
    {
        void Add(LanguageTest languageTest);
        Task SaveChangesAsync();
    }
}
