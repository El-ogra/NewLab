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

        public async Task<byte[]> GenerateAggregateAsync(int patientId, DateTime forDate)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) throw new InvalidOperationException("Patient not found");

            var tests = await _context.PatientTests
                .Include(pt => pt.LabTest)
                .Include(pt => pt.Visit)
                .Where(pt => pt.Visit.PatientId == patientId && pt.Visit.VisitDate.Date == forDate.Date)
                .ToListAsync();

            var results = await _context.TestResults
                .Where(tr => tests.Select(t => t.Id).Contains(tr.PatientTestId))
                .Include(tr => tr.Element)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text("تقرير مجمع — NewLab").Bold().FontSize(14).AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"المريض: {patient.FullName} | Lab ID: {patient.LabId ?? "N/A"} | التاريخ: {forDate:yyyy-MM-dd}");
                        col.Item().PaddingVertical(5).LineHorizontal(1);

                        foreach (var test in tests)
                        {
                            col.Item().Text($"{test.LabTest.TestName}").Bold();
                            var testResults = results.Where(r => r.PatientTestId == test.Id).ToList();
                            foreach (var r in testResults)
                            {
                                col.Item().PaddingLeft(10).Text($"{r.Element?.Name}: {r.Value} {r.Unit} {r.FlagText}");
                            }
                        }
                    });
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateWorksheetAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) throw new InvalidOperationException("Patient not found");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header().Text("ورقة عمل — NewLab").Bold().FontSize(14).AlignCenter();
                    page.Content().Text($"المريض: {patient.FullName} | Lab ID: {patient.LabId ?? "N/A"}");
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateEnvelopeAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) throw new InvalidOperationException("Patient not found");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.C6);
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        col.Item().Text(patient.FullName).Bold().FontSize(14);
                        col.Item().Text(patient.LabId ?? "");
                        col.Item().Text(patient.FileCode ?? "");
                    });
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateHistoryAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) throw new InvalidOperationException("Patient not found");

            var tests = await _context.PatientTests
                .Include(pt => pt.LabTest)
                .Include(pt => pt.Visit)
                .Where(pt => pt.Visit.PatientId == patientId)
                .OrderByDescending(pt => pt.Visit.VisitDate)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header().Text("تاريخ مرضي — NewLab").Bold().FontSize(14).AlignCenter();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"المريض: {patient.FullName}");
                        col.Item().PaddingVertical(5).LineHorizontal(1);
                        foreach (var test in tests)
                        {
                            col.Item().Text($"{test.Visit.VisitDate:yyyy-MM-dd} — {test.LabTest.TestName} — {test.Status}");
                        }
                    });
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateBlankAsync(int patientId)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("تقرير فارغ — NewLab").Bold().FontSize(14).AlignCenter();
                    page.Content().Text("تقرير بدون نتائج");
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return await Task.FromResult(ms.ToArray());
        }
    }
}
