using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<bool> LicensePlateExistsAsync(string licensePlate, int excludeHelperId);
        void Add(Car car);
        void Remove(Car car);
        Task SaveChangesAsync();
    }
}
