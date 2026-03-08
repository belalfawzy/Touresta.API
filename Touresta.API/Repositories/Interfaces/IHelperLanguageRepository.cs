using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IHelperLanguageRepository
    {
        Task<HelperLanguage?> GetByHelperAndCodeAsync(string helperId, string languageCode);
        Task<List<string>> GetLanguageCodesByHelperIdAsync(string helperId);
        Task<List<HelperLanguage>> GetByHelperIdAsync(string helperId);
        void Add(HelperLanguage helperLanguage);
        Task SaveChangesAsync();
    }
}
