using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace NewLab.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly NewLabDbContext _context;

        public AuthService(NewLabDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return user;
            }

            return null;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task<User> CreateAdminAccountAsync(string username, string password, string fullName, string email, string phone)
        {
            // Check if username already exists
            if (_context.Users.Any(u => u.Username == username))
            {
                throw new Exception("Username already exists");
            }

            // Create user
            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                FullName = fullName,
                Email = email,
                PhoneNumber = phone,
                CreatedAt = DateTime.Now
            };

            // Assign Admin role
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
            {
                throw new Exception("Admin role not found");
            }

            user.UserRoles.Add(new UserRole { RoleId = adminRole.Id, UserId = user.Id });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
