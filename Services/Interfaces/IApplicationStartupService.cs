using System.Threading.Tasks;

namespace NewLab.Services.Interfaces
{
    public interface IApplicationStartupService
    {
        /// <summary>
        /// Checks if the database is empty (no users exist).
        /// </summary>
        Task<bool> IsFirstRunAsync();

        /// <summary>
        /// Ensures default roles (Admin, Technician, Receptionist) exist in the database.
        /// </summary>
        Task SeedDefaultRolesAsync();
    }
}
