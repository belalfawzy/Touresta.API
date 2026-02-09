namespace Touresta.API.DTOs.Car
{
    /// <summary>Car information response.</summary>
    public class CarResponse
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string EnergyType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CarLicenseFile { get; set; } = string.Empty;
        public string PersonalLicenseFile { get; set; } = string.Empty;
    }
}
