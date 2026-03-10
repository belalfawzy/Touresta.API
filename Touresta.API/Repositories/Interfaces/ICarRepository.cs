using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<bool> LicensePlateExistsAsync(string licensePlate, string excludeHelperId);
        void Add(Car car);
        void Remove(Car car);
        Task SaveChangesAsync();
    }
}
