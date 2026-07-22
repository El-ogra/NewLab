using System;
using Microsoft.EntityFrameworkCore;
using NewLab.Models.Domain;

namespace NewLab.Data
{
    public class NewLabDbContext : DbContext
    {
        public NewLabDbContext(DbContextOptions<NewLabDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<SpecimenType> SpecimenTypes { get; set; }
        public DbSet<PatientVisit> PatientVisits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configure UserRole composite primary key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // 2. Configure relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // 3. Ensure Username is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // 4. Seed default roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "System Administrator" },
                new Role { Id = 2, Name = "Technician", Description = "Lab Technician" },
                new Role { Id = 3, Name = "Receptionist", Description = "Front Desk Receptionist" }
            );

            // 5. Patient relationships
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Referral)
                .WithMany()
                .HasForeignKey(p => p.ReferralId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.ExternalSpecimenType)
                .WithMany()
                .HasForeignKey(p => p.ExternalSpecimenTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. PatientVisit relationship
            modelBuilder.Entity<PatientVisit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 7. Indexes
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.LabId)
                .IsUnique()
                .HasFilter("[LabId] IS NOT NULL");

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.FileCode)
                .IsUnique();

            // 8. Decimal precision
            modelBuilder.Entity<Patient>()
                .Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Patient>()
                .Property(p => p.PaidAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Patient>()
                .Property(p => p.DiscountValue)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Referral>()
                .Property(r => r.DiscountPercent)
                .HasColumnType("decimal(5,2)");

            // 9. Seed default referral (المعمل)
            modelBuilder.Entity<Referral>().HasData(
                new Referral
                {
                    Id = 1,
                    Name = "المعمل",
                    DiscountPercent = 0,
                    IsDefaultLab = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );
        }
    }
}
