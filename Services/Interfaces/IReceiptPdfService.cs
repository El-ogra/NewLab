using System.Threading.Tasks;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface IReceiptPdfService
    {
        Task<byte[]> GenerateReceiptAsync(Patient patient, System.Collections.Generic.List<PatientTestRow> tests, ReceiptSettings settings);
    }
}
