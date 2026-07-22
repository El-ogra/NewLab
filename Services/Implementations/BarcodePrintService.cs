using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using NewLab.Helpers;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NewLab.Services.Implementations
{
    public class BarcodePrintService : IBarcodePrintService
    {
        public byte[] GeneratePdf(IEnumerable<BarcodeLabel> labels, BarcodeSettings settings)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                foreach (var label in labels)
                {
                    container.Page(page =>
                    {
                        page.Size(settings.LabelWidth, settings.LabelHeight, Unit.Millimetre);
                        page.Margin(2, Unit.Millimetre);

                        page.Content().Column(column =>
                        {
                            column.Item().Text(label.PatientName).FontSize(6).Bold();

                            if (!string.IsNullOrEmpty(label.SpecimenName))
                            {
                                column.Item().Text(label.SpecimenName).FontSize(5);
                            }

                            if (label.Tests.Any())
                            {
                                column.Item().Text(string.Join(", ", label.Tests)).FontSize(4);
                            }

                            var barcodeImage = BarcodeImageGenerator.GenerateCode128(label.Code, 200, 50);
                            column.Item().Image(barcodeImage.ToBytes()).FitWidth();

                            column.Item().Text(label.Code).FontSize(5).AlignCenter();
                        });
                    });
                }
            }).GeneratePdf();
        }
    }

    internal static class BitmapSourceExtensions
    {
        public static byte[] ToBytes(this BitmapSource bitmapSource)
        {
            using var stream = new System.IO.MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);
            return stream.ToArray();
        }
    }
}
