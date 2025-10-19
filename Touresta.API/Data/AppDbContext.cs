using Microsoft.EntityFrameworkCore;
using Touresta.API.Models;

namespace Touresta.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Helper> Helpers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Admin> Admins { get; set; }

    }
}
