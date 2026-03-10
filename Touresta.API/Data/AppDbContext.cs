using Microsoft.EntityFrameworkCore;
using RAFIQ.API.Models;

namespace RAFIQ.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Helper> Helpers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<HelperLanguage> HelperLanguages { get; set; }
        public DbSet<LanguageTest> LanguageTests { get; set; }
        public DbSet<DrugTest> DrugTests { get; set; }
        public DbSet<AdminAuditLog> AdminAuditLogs { get; set; }
        public DbSet<HelperReport> HelperReports { get; set; }
        public DbSet<AdminNote> AdminNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── User ───────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Id).HasMaxLength(32);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.GoogleId);
                entity.HasIndex(u => u.UserId).IsUnique();
            });

            // ─── Admin ──────────────────────────────────────────────────
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(a => a.Id).HasMaxLength(32);
            });

            // ─── Helper ─────────────────────────────────────────────────
            modelBuilder.Entity<Helper>(entity =>
            {
                entity.Property(h => h.Id).HasMaxLength(32);
                entity.Property(h => h.UserId).HasMaxLength(32);
                entity.Property(h => h.BannedByAdminId).HasMaxLength(32);
                entity.Property(h => h.SuspendedByAdminId).HasMaxLength(32);
                entity.Property(h => h.ReviewedByAdminId).HasMaxLength(32);

                entity.HasIndex(h => h.HelperId).IsUnique();
                entity.HasIndex(h => h.UserId).IsUnique();

                entity.HasIndex(h => h.ApprovalStatus);
                entity.HasIndex(h => h.IsApproved);
                entity.HasIndex(h => h.IsActive);
                entity.HasIndex(h => h.IsBanned);
                entity.HasIndex(h => h.IsSuspended);

                entity.HasOne(h => h.User)
                    .WithOne(u => u.Helper)
                    .HasForeignKey<Helper>(h => h.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── Car ────────────────────────────────────────────────────
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(c => c.Id).HasMaxLength(32);
                entity.Property(c => c.HelperId).HasMaxLength(32);

                entity.HasIndex(c => c.HelperId).IsUnique();
                entity.HasIndex(c => c.LicensePlate).IsUnique();

                entity.HasOne(c => c.Helper)
                    .WithOne(h => h.Car)
                    .HasForeignKey<Car>(c => c.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Certificate ────────────────────────────────────────────
            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.Property(c => c.Id).HasMaxLength(32);
                entity.Property(c => c.HelperId).HasMaxLength(32);

                entity.HasOne(c => c.Helper)
                    .WithMany(h => h.Certificates)
                    .HasForeignKey(c => c.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── HelperLanguage ─────────────────────────────────────────
            modelBuilder.Entity<HelperLanguage>(entity =>
            {
                entity.Property(hl => hl.Id).HasMaxLength(32);
                entity.Property(hl => hl.HelperId).HasMaxLength(32);

                entity.HasIndex(hl => new { hl.HelperId, hl.LanguageCode }).IsUnique();

                entity.HasOne(hl => hl.Helper)
                    .WithMany(h => h.Languages)
                    .HasForeignKey(hl => hl.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(hl => hl.AiScore)
                    .HasPrecision(5, 2);
            });

            // ─── LanguageTest ───────────────────────────────────────────
            modelBuilder.Entity<LanguageTest>(entity =>
            {
                entity.Property(lt => lt.Id).HasMaxLength(32);
                entity.Property(lt => lt.HelperLanguageId).HasMaxLength(32);

                entity.HasOne(lt => lt.HelperLanguage)
                    .WithMany(hl => hl.TestHistory)
                    .HasForeignKey(lt => lt.HelperLanguageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(lt => lt.AiScore)
                    .HasPrecision(5, 2);
            });

            // ─── DrugTest ───────────────────────────────────────────────
            modelBuilder.Entity<DrugTest>(entity =>
            {
                entity.Property(dt => dt.Id).HasMaxLength(32);
                entity.Property(dt => dt.HelperId).HasMaxLength(32);

                entity.HasIndex(dt => new { dt.HelperId, dt.IsCurrent });

                entity.HasOne(dt => dt.Helper)
                    .WithMany(h => h.DrugTests)
                    .HasForeignKey(dt => dt.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── AdminAuditLog ──────────────────────────────────────────
            modelBuilder.Entity<AdminAuditLog>(entity =>
            {
                entity.Property(a => a.Id).HasMaxLength(32);
                entity.Property(a => a.AdminId).HasMaxLength(32);
                entity.Property(a => a.TargetId).HasMaxLength(32);

                entity.HasIndex(a => a.AdminId);
                entity.HasIndex(a => new { a.TargetType, a.TargetId });
                entity.HasIndex(a => a.Timestamp);
            });

            // ─── HelperReport ───────────────────────────────────────────
            modelBuilder.Entity<HelperReport>(entity =>
            {
                entity.Property(r => r.Id).HasMaxLength(32);
                entity.Property(r => r.HelperId).HasMaxLength(32);
                entity.Property(r => r.UserId).HasMaxLength(32);
                entity.Property(r => r.ResolvedByAdminId).HasMaxLength(32);

                entity.HasIndex(r => r.HelperId);
                entity.HasIndex(r => r.UserId);
                entity.HasIndex(r => r.IsResolved);

                entity.HasOne(r => r.Helper)
                    .WithMany()
                    .HasForeignKey(r => r.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── AdminNote ──────────────────────────────────────────────
            modelBuilder.Entity<AdminNote>(entity =>
            {
                entity.Property(n => n.Id).HasMaxLength(32);
                entity.Property(n => n.HelperId).HasMaxLength(32);
                entity.Property(n => n.AdminId).HasMaxLength(32);

                entity.HasIndex(n => n.HelperId);
                entity.HasIndex(n => n.AdminId);

                entity.HasOne(n => n.Helper)
                    .WithMany()
                    .HasForeignKey(n => n.HelperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
