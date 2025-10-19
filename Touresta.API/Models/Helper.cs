using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class Helper
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string NationalIdPhoto { get; set; } 
        public string DrugTestFile { get; set; } 
        public DateTime DrugTestExpiry { get; set; } 

        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string PasswordHash { get; set; }


        public List<string> Languages { get; set; } = new();

        public bool HasCar { get; set; }
        public bool IsActive { get; set; } = true;

        public Car? Car { get; set; } 
        public List<Certificate> Certificates { get; set; } = new();
    }
}
