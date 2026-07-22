using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface IBarcodeService
    {
        string GenerateCaseCode(Patient patient, PatientVisit visit);
        string GenerateFileCode(Patient patient);
        string GenerateLabCode(Patient patient);
        Task<Patient> GetOrCreateLabIdAsync(Patient patient);
        Task<List<BarcodeLabel>> GetLabelsForPatientAsync(int patientId);
        Task<BarcodeSettings> GetSettingsAsync();
        Task SaveSettingsAsync(BarcodeSettings settings);
    }
}
