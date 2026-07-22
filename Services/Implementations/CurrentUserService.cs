using System.Linq;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        public User? CurrentUser { get; private set; }

        public bool IsAdmin => CurrentUser?.UserRoles
            .Any(ur => ur.Role?.Name == "Admin") ?? false;

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }

        public void Clear()
        {
            CurrentUser = null;
        }
    }
}
