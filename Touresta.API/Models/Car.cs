using Touresta.API.Enums.Car;

namespace Touresta.API.Models
{
    public class Car
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public CarColor Color { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public CarEnergyType EnergyType { get; set; }
        public string CarLicenseFile { get; set; } = string.Empty;
        public string PersonalLicenseFile { get; set; } = string.Empty;
        public CarType Type { get; set; }

        public string HelperId { get; set; } = string.Empty;
        public Helper Helper { get; set; } = null!;
    }
}
