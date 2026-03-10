using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface IDrugTestRepository
    {
        void Add(DrugTest drugTest);
        Task SaveChangesAsync();
    }
}
