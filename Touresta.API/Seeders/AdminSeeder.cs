using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Touresta.API.Data;
using Touresta.API.Enums;
using Touresta.API.Models;

namespace Touresta.API.Seeders
{
    public static class AdminSeeder
    {
        public static void Seed(AppDbContext context, IServiceProvider services)
        {
            var hasher = new PasswordHasher<Admin>();

            string email = "belalfawzy321@gmail.com";
            string password = "Admin@123";

            var admin = context.Admins.FirstOrDefault(a => a.Email == email);

            if (admin == null)
            {
                admin = new Admin
                {
                    FullName = "Main Admin",
                    Email = email,
                    PasswordHash = hasher.HashPassword(null, password),
                    Role = Role.SuperAdmin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                context.Admins.Add(admin);
            }
            else
            {
                admin.PasswordHash = hasher.HashPassword(admin, password);
                context.Admins.Update(admin);
            }

            context.SaveChanges();
        }
    }
}
