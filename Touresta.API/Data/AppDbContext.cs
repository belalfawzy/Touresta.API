using Microsoft.EntityFrameworkCore;
using Touresta.API.Models;

namespace Touresta.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Helper> Helpers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<HelperLanguage> HelperLanguages { get; set; }
        public DbSet<LanguageTest> LanguageTests { get; set; }
        public DbSet<DrugTest> DrugTests { get; set; }
        public DbSet<AdminAuditLog> AdminAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── User ───
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.GoogleId);
                entity.HasIndex(u => u.UserId).IsUnique();
            });

            // ─── Helper (1:1 with User) ───
            modelBuilder.Entity<Helper>(entity =>
            {
                entity.HasIndex(h => h.HelperId).IsUnique();
                entity.HasIndex(h => h.UserId).IsUnique();

                entity.HasOne(h => h.User)
                    .WithOne(u => u.Helper)
                    .HasForeignKey<Helper>(h => h.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── Car (1:1 with Helper) ───
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasIndex(c => c.HelperId).IsUnique();
                entity.HasIndex(c => c.LicensePlate).IsUnique();

                entity.HasOne(c => c.Helper)
                    .WithOne(h => h.Car)
                    .HasForeignKey<Car>(c => c.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Certificate (many per Helper) ───
            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasOne(c => c.Helper)
                    .WithMany(h => h.Certificates)
                    .HasForeignKey(c => c.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── HelperLanguage (many per Helper) ───
            modelBuilder.Entity<HelperLanguage>(entity =>
            {
                entity.HasIndex(hl => new { hl.HelperId, hl.LanguageCode }).IsUnique();

                entity.HasOne(hl => hl.Helper)
                    .WithMany(h => h.Languages)
                    .HasForeignKey(hl => hl.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(hl => hl.AiScore)
                    .HasPrecision(5, 2);
            });

            // ─── LanguageTest (many per HelperLanguage) ───
            modelBuilder.Entity<LanguageTest>(entity =>
            {
                entity.HasOne(lt => lt.HelperLanguage)
                    .WithMany(hl => hl.TestHistory)
                    .HasForeignKey(lt => lt.HelperLanguageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(lt => lt.AiScore)
                    .HasPrecision(5, 2);
            });

            // ─── DrugTest (many per Helper) ───
            modelBuilder.Entity<DrugTest>(entity =>
            {
                entity.HasIndex(dt => new { dt.HelperId, dt.IsCurrent });

                entity.HasOne(dt => dt.Helper)
                    .WithMany(h => h.DrugTests)
                    .HasForeignKey(dt => dt.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── AdminAuditLog ───
            modelBuilder.Entity<AdminAuditLog>(entity =>
            {
                entity.HasIndex(a => a.AdminId);
                entity.HasIndex(a => new { a.TargetType, a.TargetId });
                entity.HasIndex(a => a.Timestamp);
            });
        }
    }
}
