using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;
using System.Threading.Tasks;

namespace NewLab.Services.Implementations
{
    public class ApplicationStartupService : IApplicationStartupService
    {
        private readonly NewLabDbContext _context;

        public ApplicationStartupService(NewLabDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsFirstRunAsync()
        {
            // Check if Users table is empty
            return !await _context.Users.AnyAsync();
        }

        public async Task SeedDefaultRolesAsync()
        {
            // Ensure default roles exist
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin", Description = "System Administrator" },
                new Role { Id = 2, Name = "Technician", Description = "Lab Technician" },
                new Role { Id = 3, Name = "Receptionist", Description = "Front Desk Receptionist" }
            };

            foreach (var role in roles)
            {
                if (!_context.Roles.Any(r => r.Id == role.Id))
                {
                    _context.Roles.Add(role);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
