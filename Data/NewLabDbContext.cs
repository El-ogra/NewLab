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
        }
    }
}
