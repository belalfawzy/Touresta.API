using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class DrugTestRepository : IDrugTestRepository
    {
        private readonly AppDbContext _db;

        public DrugTestRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Add(DrugTest drugTest)
            => _db.DrugTests.Add(drugTest);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
