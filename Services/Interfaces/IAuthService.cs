using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Validates username and password against the database using BCrypt.
        /// Updates LastLoginAt on success.
        /// </summary>
        Task<User?> ValidateCredentialsAsync(string username, string password);

        /// <summary>
        /// Hashes a plain text password using BCrypt.
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain text password against a stored BCrypt hash.
        /// </summary>
        bool VerifyPassword(string password, string hash);

        /// <summary>
        /// Creates the initial Admin account and assigns the "Admin" role.
        /// </summary>
        Task<User> CreateAdminAccountAsync(string username, string password, string fullName, string email, string phone);
    }
}
