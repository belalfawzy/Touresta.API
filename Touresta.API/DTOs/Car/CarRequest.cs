using Touresta.API.Enums.Car;

namespace Touresta.API.DTOs.Car
{
    /// <summary>Request to add or update car information.</summary>
    public class CarRequest
    {
        /// <summary>Vehicle manufacturer.</summary>
        /// <example>Toyota</example>
        public string Brand { get; set; } = string.Empty;

        /// <summary>Vehicle model.</summary>
        /// <example>Corolla</example>
        public string Model { get; set; } = string.Empty;

        /// <summary>Vehicle color.</summary>
        /// <example>White</example>
        public CarColor Color { get; set; }

        /// <summary>Vehicle registration number.</summary>
        /// <example>ABC-1234</example>
        public string LicensePlate { get; set; } = string.Empty;

        /// <summary>Energy type.</summary>
        /// <example>Gasoline</example>
        public CarEnergyType EnergyType { get; set; }

        /// <summary>Vehicle type.</summary>
        /// <example>Sedan</example>
        public CarType Type { get; set; }
    }
}
