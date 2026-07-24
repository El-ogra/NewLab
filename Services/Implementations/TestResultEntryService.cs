using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;
using NewLab.Services.Interfaces;

namespace NewLab.Services.Implementations
{
    public class TestResultEntryService : ITestResultEntryService
    {
        private readonly NewLabDbContext _context;
        private readonly INormalRangeService _normalRangeService;
        private readonly IReportPdfGenerator _reportGenerator;
        private readonly ICurrentUserService _currentUserService;

        public TestResultEntryService(
            NewLabDbContext context,
            INormalRangeService normalRangeService,
            IReportPdfGenerator reportGenerator,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _normalRangeService = normalRangeService;
            _reportGenerator = reportGenerator;
            _currentUserService = currentUserService;
        }

        public async Task<(PatientTest patientTest, LabTest labTest, List<LabTestElement> elements, List<TestResult> existingResults)> GetPatientTestWithProfileAsync(int patientTestId)
        {
            var patientTest = await _context.PatientTests
                .Include(pt => pt.LabTest)
                .ThenInclude(lt => lt.Elements)
                .Include(pt => pt.Visit)
                .ThenInclude(v => v.Patient)
                .FirstOrDefaultAsync(pt => pt.Id == patientTestId);

            if (patientTest == null)
                throw new InvalidOperationException("PatientTest not found");

            var elements = patientTest.LabTest.Elements.OrderBy(e => e.ArrangeNumber).ToList();

            var existingResults = await _context.TestResults
                .Where(tr => tr.PatientTestId == patientTestId)
                .ToListAsync();

            return (patientTest, patientTest.LabTest, elements, existingResults);
        }

        public async Task SaveResultsAsync(int patientTestId, IEnumerable<TestResult> results, string? comment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = await _context.TestResults
                    .Where(tr => tr.PatientTestId == patientTestId)
                    .ToListAsync();
                _context.TestResults.RemoveRange(existing);

                foreach (var result in results)
                {
                    result.PatientTestId = patientTestId;
                    result.EnteredAt = DateTime.Now;
                    _context.TestResults.Add(result);
                }

                var patientTest = await _context.PatientTests.FindAsync(patientTestId);
                if (patientTest != null)
                {
                    patientTest.Status = TestStatus.Entered;
                    patientTest.EnteredByUserId = _currentUserService.CurrentUser?.Id;
                    patientTest.EnteredAt = DateTime.Now;

                    _context.AuditLogs.Add(new AuditLog
                    {
                        EntityName = "PatientTest",
                        EntityId = patientTestId,
                        Action = "Enter",
                        UserId = _currentUserService.CurrentUser?.Id ?? 0,
                        Timestamp = DateTime.Now,
                        Details = comment
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<NormalRangeEvaluation?> EvaluateResultAsync(decimal value, int labTestId, Patient patient)
        {
            var range = await _normalRangeService.GetMatchingRangeAsync(labTestId, patient);
            if (range == null) return null;
            return await _normalRangeService.EvaluateValueAsync(range, value);
        }

        public async Task<List<SavedComment>> GetSavedCommentsAsync(int labTestId)
        {
            return await _context.SavedComments
                .Where(sc => sc.LabTestId == labTestId)
                .ToListAsync();
        }

        public async Task MarkReviewedAsync(int patientTestId, bool isReviewed)
        {
            var pt = await _context.PatientTests.FindAsync(patientTestId);
            if (pt != null)
            {
                pt.IsReviewed = isReviewed;
                pt.ReviewedByUserId = isReviewed ? _currentUserService.CurrentUser?.Id : null;
                pt.Status = isReviewed ? TestStatus.Reviewed : TestStatus.Entered;

                _context.AuditLogs.Add(new AuditLog
                {
                    EntityName = "PatientTest",
                    EntityId = patientTestId,
                    Action = isReviewed ? "Review" : "Unreview",
                    UserId = _currentUserService.CurrentUser?.Id ?? 0,
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task<byte[]> PreviewReportAsync(int patientTestId)
        {
            return await _reportGenerator.GenerateAsync(patientTestId);
        }

        public async Task<byte[]> PrintReportAsync(int patientTestId)
        {
            var pdf = await _reportGenerator.GenerateAsync(patientTestId);

            var pt = await _context.PatientTests.FindAsync(patientTestId);
            if (pt != null)
            {
                pt.IsPrinted = true;
                pt.PrintedByUserId = _currentUserService.CurrentUser?.Id;
                pt.Status = TestStatus.Printed;

                _context.AuditLogs.Add(new AuditLog
                {
                    EntityName = "PatientTest",
                    EntityId = patientTestId,
                    Action = "Print",
                    UserId = _currentUserService.CurrentUser?.Id ?? 0,
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }

            return pdf;
        }
    }
}
