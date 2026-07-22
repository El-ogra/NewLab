using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface ITestResultEntryService
    {
        Task<(PatientTest patientTest, LabTest labTest, List<LabTestElement> elements, List<TestResult> existingResults)> GetPatientTestWithProfileAsync(int patientTestId);
        Task SaveResultsAsync(int patientTestId, IEnumerable<TestResult> results, string? comment);
        Task<NormalRangeEvaluation?> EvaluateResultAsync(decimal value, int labTestId, Patient patient);
        Task<List<SavedComment>> GetSavedCommentsAsync(int labTestId);
        Task MarkReviewedAsync(int patientTestId);
        Task<byte[]> PreviewReportAsync(int patientTestId);
        Task<byte[]> PrintReportAsync(int patientTestId);
    }
}
