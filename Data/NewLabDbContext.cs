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
        public DbSet<TestGroup> TestGroups { get; set; }
        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<LabTestElement> LabTestElements { get; set; }
        public DbSet<ReferralPrice> ReferralPrices { get; set; }
        public DbSet<NormalRange> NormalRanges { get; set; }
        public DbSet<BarcodeSettings> BarcodeSettings { get; set; }
        public DbSet<PatientCode> PatientCodes { get; set; }

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

            // 10. LabTest configurations
            modelBuilder.Entity<LabTest>()
                .HasIndex(l => l.Code)
                .IsUnique();

            modelBuilder.Entity<LabTest>()
                .HasOne(l => l.TestGroup)
                .WithMany(g => g.Tests)
                .HasForeignKey(l => l.TestGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabTest>()
                .HasOne(l => l.ParentLabTest)
                .WithMany()
                .HasForeignKey(l => l.ParentLabTestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabTest>()
                .HasOne(l => l.DefaultSpecimenType)
                .WithMany()
                .HasForeignKey(l => l.DefaultSpecimenTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabTest>()
                .HasOne(l => l.ExternalReferral)
                .WithMany()
                .HasForeignKey(l => l.ExternalReferralId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabTest>()
                .Property(l => l.PatientPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<LabTest>()
                .Property(l => l.LabToLabPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<LabTest>()
                .Property(l => l.ExternalCost)
                .HasColumnType("decimal(18,2)");

            // 11. LabTestElement configurations
            modelBuilder.Entity<LabTestElement>()
                .HasOne(e => e.ParentLabTest)
                .WithMany(t => t.Elements)
                .HasForeignKey(e => e.ParentLabTestId)
                .OnDelete(DeleteBehavior.Cascade);

            // 12. ReferralPrice configurations
            modelBuilder.Entity<ReferralPrice>()
                .HasOne(rp => rp.LabTest)
                .WithMany(t => t.ReferralPrices)
                .HasForeignKey(rp => rp.LabTestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReferralPrice>()
                .HasOne(rp => rp.Referral)
                .WithMany()
                .HasForeignKey(rp => rp.ReferralId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReferralPrice>()
                .HasIndex(rp => new { rp.LabTestId, rp.ReferralId })
                .IsUnique();

            modelBuilder.Entity<ReferralPrice>()
                .Property(rp => rp.Price)
                .HasColumnType("decimal(18,2)");

            // 13. NormalRange configurations
            modelBuilder.Entity<NormalRange>()
                .HasOne(nr => nr.LabTest)
                .WithMany()
                .HasForeignKey(nr => nr.LabTestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NormalRange>()
                .HasIndex(nr => new { nr.LabTestId, nr.Gender, nr.AgeFrom, nr.AgeTo, nr.AgeUnit });

            modelBuilder.Entity<NormalRange>().Property(nr => nr.LowLimit).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<NormalRange>().Property(nr => nr.HighLimit).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<NormalRange>().Property(nr => nr.CriticalLowLimit).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<NormalRange>().Property(nr => nr.CriticalHighLimit).HasColumnType("decimal(18,4)");

            // 14. BarcodeSettings — single row seeded with defaults (Decision 4)
            modelBuilder.Entity<BarcodeSettings>().HasData(
                new BarcodeSettings
                {
                    Id = 1,
                    OffsetX = 0,
                    OffsetY = 0,
                    PrintFileCodeWithAll = false,
                    LabelWidth = 38,
                    LabelHeight = 25
                }
            );

            // 15. PatientCode configurations
            modelBuilder.Entity<PatientCode>()
                .HasOne(pc => pc.Patient)
                .WithMany()
                .HasForeignKey(pc => pc.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PatientCode>()
                .HasIndex(pc => new { pc.PatientId, pc.CodeType })
                .IsUnique();

            modelBuilder.Entity<PatientCode>()
                .HasIndex(pc => pc.CodeValue);

            // 16. Seed TestGroups
            modelBuilder.Entity<TestGroup>().HasData(
                new TestGroup { Id = 1, Name = "Chemistry", LogGroup = "CHEM" },
                new TestGroup { Id = 2, Name = "Hematology", LogGroup = "HEM" },
                new TestGroup { Id = 3, Name = "Urine", LogGroup = "URI" }
            );

            // 17. Seed LabTests
            modelBuilder.Entity<LabTest>().HasData(
                new LabTest
                {
                    Id = 1,
                    Code = "GLU",
                    TestName = "Glucose",
                    ReportNameLarge = "Glucose",
                    ArabicName = "سكر",
                    TestGroupId = 1,
                    PatientPrice = 10m,
                    LabToLabPrice = 30m,
                    IsRoutine = true,
                    IsActive = true,
                    TestTimeDays = 0,
                    ArrangeNumber = 1
                },
                new LabTest
                {
                    Id = 2,
                    Code = "HGB",
                    TestName = "Hemoglobin",
                    ReportNameLarge = "Hemoglobin",
                    ArabicName = "هموغلوبين",
                    TestGroupId = 2,
                    PatientPrice = 15m,
                    LabToLabPrice = 35m,
                    IsRoutine = true,
                    IsActive = true,
                    TestTimeDays = 0,
                    ArrangeNumber = 2
                },
                new LabTest
                {
                    Id = 3,
                    Code = "UAC",
                    TestName = "Urine Analysis",
                    ReportNameLarge = "Urine Analysis",
                    ArabicName = "تحليل بول",
                    TestGroupId = 3,
                    PatientPrice = 8m,
                    LabToLabPrice = 20m,
                    IsRoutine = true,
                    IsActive = true,
                    TestTimeDays = 0,
                    ArrangeNumber = 3
                }
            );
        }
    }
}
