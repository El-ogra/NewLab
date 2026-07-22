using System.Threading.Tasks;

namespace NewLab.Services.Interfaces
{
    public interface IReportPdfGenerator
    {
        Task<byte[]> GenerateAsync(int patientTestId);
    }
}
