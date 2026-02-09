using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class CarRepository : ICarRepository
    {
        private readonly AppDbContext _db;

        public CarRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> LicensePlateExistsAsync(string licensePlate, int excludeHelperId)
            => await _db.Cars.AnyAsync(c => c.LicensePlate == licensePlate && c.HelperId != excludeHelperId);

        public void Add(Car car)
            => _db.Cars.Add(car);

        public void Remove(Car car)
            => _db.Cars.Remove(car);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
