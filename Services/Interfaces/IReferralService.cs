using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface IReferralService
    {
        Task<List<Referral>> SearchByNameAsync(string prefix);
        Task<Referral> GetOrCreateAsync(string name);
        Task<Referral> GetDefaultLabAsync();
    }
}
