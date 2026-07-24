using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NewLab.Services.Implementations
{
    public class ReceiptPdfService : IReceiptPdfService
    {
        public Task<byte[]> GenerateReceiptAsync(Patient patient, List<PatientTestRow> tests, ReceiptSettings settings)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(20);

                    page.Header().Text("إيصال المعمل").FontSize(18).Bold().AlignCenter();

                    page.Content().Column(column =>
                    {
                        column.Item().PaddingVertical(5).LineHorizontal(1);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"المريض: {patient.FullName}");
                            row.RelativeItem().AlignRight().Text($"التاريخ: {patient.CreatedAt:yyyy-MM-dd}");
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"السن: {patient.AgeValue} {patient.AgeUnit}");
                            row.RelativeItem().AlignRight().Text($"الجنس: {patient.Gender}");
                        });

                        if (!string.IsNullOrEmpty(patient.LabId))
                        {
                            column.Item().Text($"Lab ID: {patient.LabId}");
                        }

                        column.Item().PaddingVertical(5).LineHorizontal(1);

                        if (settings.ShowTestsDetails && tests.Count > 0)
                        {
                            column.Item().Text("التحاليل:").Bold();
                            foreach (var test in tests)
                            {
                                column.Item().PaddingLeft(10).Text($"• {test.TestName} — {test.Price:C}");
                            }
                            column.Item().PaddingVertical(5).LineHorizontal(1);
                        }

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("الإجمالي:").Bold();
                            row.RelativeItem().AlignRight().Text($"{patient.TotalAmount:C}").Bold();
                        });

                        if (patient.DiscountValue > 0)
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text("الخصم:");
                                row.RelativeItem().AlignRight().Text($"-{patient.DiscountValue:C}");
                            });
                        }

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("المدفوع:").Bold();
                            row.RelativeItem().AlignRight().Text($"{patient.PaidAmount:C}");
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("المتبقي:").Bold();
                            row.RelativeItem().AlignRight().Text($"{patient.TotalAmount - patient.PaidAmount:C}");
                        });

                        column.Item().PaddingVertical(10);
                        column.Item().AlignCenter().Text("شكراً لثقتكم").FontSize(12).Italic();
                    });
                });
            });

            var pdf = document.GeneratePdf();
            return Task.FromResult(pdf);
        }
    }
}
