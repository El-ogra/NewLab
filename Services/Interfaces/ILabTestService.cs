using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface ILabTestService
    {
        Task<List<LabTest>> GetAllAsync();
        Task<List<LabTest>> SearchAsync(string? code, string? groupName, string? testName);
        Task<LabTest?> GetByIdAsync(int labTestId);
        Task<LabTest> AddAsync(LabTest labTest);
        Task<LabTest> UpdateAsync(LabTest labTest);
        Task DeleteAsync(int labTestId);
        Task<List<LabTestElement>> GetElementsAsync(int parentLabTestId);
        Task<List<LabTest>> GetRoutineTestsAsync();
        Task<List<LabTest>> GetByGroupAsync(int testGroupId);
        Task<List<ReferralPrice>> GetReferralPricesAsync(int labTestId);
        Task SetReferralPriceAsync(int labTestId, int referralId, decimal price);
        Task DeleteReferralPriceAsync(int labTestId, int referralId);
        Task<List<SpecimenType>> GetSpecimenTypesAsync();
    }
}
