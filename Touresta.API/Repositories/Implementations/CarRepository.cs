using Microsoft.EntityFrameworkCore;
using RAFIQ.API.Data;
using RAFIQ.API.Models;
using RAFIQ.API.Repositories.Interfaces;

namespace RAFIQ.API.Repositories.Implementations
{
    public class CarRepository : ICarRepository
    {
        private readonly AppDbContext _db;

        public CarRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> LicensePlateExistsAsync(string licensePlate, string excludeHelperId)
            => await _db.Cars.AnyAsync(c => c.LicensePlate == licensePlate && c.HelperId != excludeHelperId);

        public void Add(Car car)
            => _db.Cars.Add(car);

        public void Remove(Car car)
            => _db.Cars.Remove(car);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
