using System.Collections.Generic;
using NewLab.Models.Domain;

namespace NewLab.Services.Interfaces
{
    public interface IBarcodePrintService
    {
        byte[] GeneratePdf(IEnumerable<BarcodeLabel> labels, BarcodeSettings settings);
    }
}
