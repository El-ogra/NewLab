using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface IReceiptSettingsService
    {
        Task<ReceiptSettings> GetAsync();
    }
}
