using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public sealed record NormalRangeEvaluation(
        string Category,
        string? Flag);

    public interface INormalRangeService
    {
        Task<List<NormalRange>> GetForTestAsync(int labTestId);
        Task<NormalRange> AddAsync(NormalRange range);
        Task<NormalRange> UpdateAsync(NormalRange range);
        Task DeleteAsync(int rangeId);
        Task<NormalRange?> GetMatchingRangeAsync(int labTestId, Patient patient);
        Task<NormalRangeEvaluation> EvaluateValueAsync(NormalRange range, decimal value);
    }
}
