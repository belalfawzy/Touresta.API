using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Touresta.API.Enums.Car;

namespace Touresta.API.DTOs.Car
{
    /// <summary>Request to add or update car information.</summary>
    public class CarRequest
    {
        /// <summary>Vehicle manufacturer.</summary>
        /// <example>Toyota</example>
        [Required(ErrorMessage = "Brand is required.")]
        [StringLength(50, ErrorMessage = "Brand cannot exceed 50 characters.")]
        public string Brand { get; set; } = string.Empty;

        /// <summary>Vehicle model.</summary>
        /// <example>Corolla</example>
        [Required(ErrorMessage = "Model is required.")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters.")]
        public string Model { get; set; } = string.Empty;

        /// <summary>Vehicle color.</summary>
        /// <example>White</example>
        [Required(ErrorMessage = "Color is required.")]
        public CarColor Color { get; set; }

        /// <summary>Vehicle registration number.</summary>
        /// <example>ABC-1234</example>
        [Required(ErrorMessage = "License plate is required.")]
        [StringLength(20, ErrorMessage = "License plate cannot exceed 20 characters.")]
        public string LicensePlate { get; set; } = string.Empty;

        /// <summary>Energy type.</summary>
        /// <example>Gasoline</example>
        [Required(ErrorMessage = "Energy type is required.")]
        public CarEnergyType EnergyType { get; set; }

        /// <summary>Vehicle type.</summary>
        /// <example>Sedan</example>
        [Required(ErrorMessage = "Vehicle type is required.")]
        public CarType Type { get; set; }

        /// <summary>Vehicle registration document (JPG/PNG/PDF, max 10MB).</summary>
        [Required(ErrorMessage = "Car license file is required.")]
        public IFormFile CarLicenseFile { get; set; } = null!;

        /// <summary>Driver's license (JPG/PNG/PDF, max 10MB).</summary>
        [Required(ErrorMessage = "Personal license file is required.")]
        public IFormFile PersonalLicenseFile { get; set; } = null!;
    }
}
