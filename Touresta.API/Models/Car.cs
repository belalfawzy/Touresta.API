using Touresta.API.Enums.Car;

namespace Touresta.API.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public CarColor Color { get; set; }
        public string LicensePlate { get; set; }
        public CarEnergyType EnergyType { get; set; } // Disel, Essence , Gasoline, Electric, Hybrid
        public string CarLicenseFile { get; set; } 
        public string PersonalLicenseFile { get; set; } 
        public CarType Type { get; set; }

        public int HelperId { get; set; }
        public Helper Helper { get; set; }
    }
}
