using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;

namespace NewLab.Services.Interfaces
{
    public sealed record PatientTestRow(int LabTestId, string TestName, decimal Price);

    public interface IPatientService
    {
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task DeleteAsync(int patientId);
        Task<Patient?> GetByIdAsync(int patientId);
        Task<List<Patient>> GetTodayPatientsAsync();
        Task<Patient?> GetByLabIdAsync(string labId);
        Task<decimal> CalculateTotalAsync(IEnumerable<PatientTestRow> patientTests, BillingSystem billingSystem, Referral? referral, decimal discountValue, bool discountIsPercent);
    }
}
