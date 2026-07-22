using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NewLab.Services.Implementations
{
    public class ReportPdfGenerator : IReportPdfGenerator
    {
        private readonly NewLabDbContext _context;
        private readonly INormalRangeService _normalRangeService;
        private readonly IAutoCalculationService _autoCalcService;
        private readonly ICurrentUserService _currentUserService;

        public ReportPdfGenerator(
            NewLabDbContext context,
            INormalRangeService normalRangeService,
            IAutoCalculationService autoCalcService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _normalRangeService = normalRangeService;
            _autoCalcService = autoCalcService;
            _currentUserService = currentUserService;

            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateAsync(int patientTestId)
        {
            var patientTest = await _context.PatientTests
                .Include(pt => pt.LabTest)
                .ThenInclude(lt => lt.Elements)
                .Include(pt => pt.Visit)
                .ThenInclude(v => v.Patient)
                .FirstOrDefaultAsync(pt => pt.Id == patientTestId);

            if (patientTest == null)
                throw new InvalidOperationException("PatientTest not found");

            var results = await _context.TestResults
                .Where(tr => tr.PatientTestId == patientTestId)
                .ToListAsync();

            var patient = patientTest.Visit.Patient;
            var labTest = patientTest.LabTest;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("NewLab Laboratory").Bold().FontSize(14);
                            col.Item().Text("Test Report").FontSize(10);
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingVertical(5).LineHorizontal(1);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Patient: {patient.FullName}");
                            row.RelativeItem().Text($"Lab ID: {patient.LabId ?? "N/A"}");
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Test: {labTest.TestName}");
                            row.RelativeItem().Text($"Date: {DateTime.Now:yyyy-MM-dd}");
                        });

                        col.Item().PaddingVertical(5).LineHorizontal(1);

                        if (results.Any())
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Element").Bold();
                                    header.Cell().Text("Value").Bold();
                                    header.Cell().Text("Unit").Bold();
                                    header.Cell().Text("Flag").Bold();
                                });

                                foreach (var result in results)
                                {
                                    table.Cell().Text(result.Element?.Name ?? "N/A");
                                    table.Cell().Text(result.Value ?? "");
                                    table.Cell().Text(result.Unit ?? "");
                                    table.Cell().Text(result.FlagText ?? "").FontColor(
                                        result.IsCritical ? Colors.Red.Medium :
                                        result.IsAbnormal ? Colors.Orange.Medium : Colors.Black);
                                }
                            });
                        }

                        col.Item().PaddingVertical(5).LineHorizontal(1);

                        col.Item().Text($"Entered by: {_currentUserService?.CurrentUser?.FullName ?? "System"}");
                        col.Item().Text($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}");
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("NewLab Laboratory Management System");
                    });
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}
