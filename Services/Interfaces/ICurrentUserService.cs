using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface ICurrentUserService
    {
        User? CurrentUser { get; }

        bool IsAdmin { get; }

        void SetCurrentUser(User user);

        void Clear();
    }
}
