using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IHelperLanguageRepository
    {
        Task<HelperLanguage?> GetByHelperAndCodeAsync(int helperId, string languageCode);
        Task<List<string>> GetLanguageCodesByHelperIdAsync(int helperId);
        Task<List<HelperLanguage>> GetByHelperIdAsync(int helperId);
        void Add(HelperLanguage helperLanguage);
        Task SaveChangesAsync();
    }
}
