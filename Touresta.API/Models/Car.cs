using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string CarLicenseFile { get; set; } 
        public string PersonalLicenseFile { get; set; } 
        public CarType Type { get; set; }

        public int HelperId { get; set; }
        public Helper Helper { get; set; }
    }
}
