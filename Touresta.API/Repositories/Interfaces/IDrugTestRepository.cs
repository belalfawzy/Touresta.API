using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IDrugTestRepository
    {
        void Add(DrugTest drugTest);
        Task SaveChangesAsync();
    }
}
