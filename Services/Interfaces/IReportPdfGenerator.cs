using System.Threading.Tasks;

namespace NewLab.Services.Interfaces
{
    public interface IReportPdfGenerator
    {
        Task<byte[]> GenerateAsync(int patientTestId);
        Task<byte[]> GenerateAggregateAsync(int patientId, System.DateTime forDate);
        Task<byte[]> GenerateWorksheetAsync(int patientId);
        Task<byte[]> GenerateEnvelopeAsync(int patientId);
        Task<byte[]> GenerateHistoryAsync(int patientId);
        Task<byte[]> GenerateBlankAsync(int patientId);
    }
}
