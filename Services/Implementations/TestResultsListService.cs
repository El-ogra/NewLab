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
    public class TestResultsListService : ITestResultsListService
    {
        private readonly NewLabDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public TestResultsListService(NewLabDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<PatientListItem>> GetTodayPatientsAsync()
        {
            return await GetPatientsByFilterAsync(PatientListFilter.All, DateTime.Today);
        }

        public async Task<List<PatientListItem>> GetPatientsByFilterAsync(PatientListFilter filterMode, DateTime? forDate)
        {
            var d = forDate ?? DateTime.Today;

            var visits = await _context.PatientVisits
                .Include(v => v.Patient)
                .Include(v => v.PatientTests)
                .ThenInclude(pt => pt.LabTest)
                .Where(v => v.VisitDate.Date == d.Date)
                .ToListAsync();

            var result = new List<PatientListItem>();

            foreach (var visit in visits)
            {
                var tests = visit.PatientTests?.ToList() ?? new List<PatientTest>();

                if (!tests.Any()) continue;

                var aggregateStatus = tests.Any(t => t.Status == TestStatus.New)
                    ? TestStatus.New
                    : tests.Min(t => t.Status);

                var item = new PatientListItem(
                    visit.Patient.Id,
                    visit.Id,
                    visit.Patient.FullName,
                    aggregateStatus,
                    tests.Count,
                    visit.Patient.IsImportant,
                    visit.DailySequenceNumber,
                    visit.Patient.Gender,
                    visit.Patient.AgeValue,
                    visit.Patient.AgeUnit,
                    visit.Patient.LabId,
                    visit.Patient.FileCode,
                    visit.Patient.VisitCode,
                    visit.Patient.Notes);

                switch (filterMode)
                {
                    case PatientListFilter.Unwritten when tests.Any(t => t.Status == TestStatus.New):
                    case PatientListFilter.Unreviewed when tests.Any(t => !t.IsReviewed):
                    case PatientListFilter.Unprinted when tests.Any(t => !t.IsPrinted):
                    case PatientListFilter.Important when visit.Patient.IsImportant:
                    case PatientListFilter.All:
                        result.Add(item);
                        break;
                    case PatientListFilter.Individual:
                    case PatientListFilter.LabToLab:
                    case PatientListFilter.Referral:
                        // BillingSystem filter - simplified for now
                        result.Add(item);
                        break;
                }
            }

            return result.OrderBy(p => p.AttendanceNumber).ToList();
        }

        public async Task<List<PatientTest>> GetPatientTestsAsync(int patientId, DateTime? forDate)
        {
            var d = forDate ?? DateTime.Today;

            var visit = await _context.PatientVisits
                .Include(v => v.PatientTests)
                .ThenInclude(pt => pt.LabTest)
                .FirstOrDefaultAsync(v => v.PatientId == patientId && v.VisitDate.Date == d.Date);

            return visit?.PatientTests?.ToList() ?? new List<PatientTest>();
        }

        public async Task<PatientListItem?> SearchByCodeAsync(string code)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p =>
                    p.LabId == code ||
                    p.FileCode == code ||
                    p.VisitCode == code);

            if (patient == null) return null;

            var visit = await _context.PatientVisits
                .Include(v => v.PatientTests)
                .FirstOrDefaultAsync(v => v.PatientId == patient.Id && v.VisitDate.Date == DateTime.Today);

            if (visit == null) return null;

            var tests = visit.PatientTests?.ToList() ?? new List<PatientTest>();
            var aggregateStatus = tests.Any()
                ? (tests.Any(t => t.Status == TestStatus.New) ? TestStatus.New : tests.Min(t => t.Status))
                : TestStatus.New;

            return new PatientListItem(
                patient.Id,
                visit.Id,
                patient.FullName,
                aggregateStatus,
                tests.Count,
                patient.IsImportant,
                visit.DailySequenceNumber,
                patient.Gender,
                patient.AgeValue,
                patient.AgeUnit,
                patient.LabId,
                patient.FileCode,
                patient.VisitCode,
                patient.Notes);
        }

        public async Task<PatientListItem?> SearchByAttendanceNumberAsync(int number, DateTime? forDate)
        {
            var d = forDate ?? DateTime.Today;

            var visit = await _context.PatientVisits
                .Include(v => v.Patient)
                .Include(v => v.PatientTests)
                .FirstOrDefaultAsync(v => v.VisitDate.Date == d.Date && v.DailySequenceNumber == number);

            if (visit == null) return null;

            var tests = visit.PatientTests?.ToList() ?? new List<PatientTest>();
            var aggregateStatus = tests.Any()
                ? (tests.Any(t => t.Status == TestStatus.New) ? TestStatus.New : tests.Min(t => t.Status))
                : TestStatus.New;

            return new PatientListItem(
                visit.Patient.Id,
                visit.Id,
                visit.Patient.FullName,
                aggregateStatus,
                tests.Count,
                visit.Patient.IsImportant,
                visit.DailySequenceNumber,
                visit.Patient.Gender,
                visit.Patient.AgeValue,
                visit.Patient.AgeUnit,
                visit.Patient.LabId,
                visit.Patient.FileCode,
                visit.Patient.VisitCode,
                visit.Patient.Notes);
        }

        public async Task ToggleReviewedAsync(int patientTestId)
        {
            var pt = await _context.PatientTests.FindAsync(patientTestId);
            if (pt == null) return;

            pt.IsReviewed = !pt.IsReviewed;
            if (pt.IsReviewed)
            {
                pt.Status = TestStatus.Reviewed;
                pt.ReviewedByUserId = _currentUserService.CurrentUser?.Id;
            }
            else
            {
                pt.Status = pt.Status == TestStatus.Reviewed ? TestStatus.Entered : pt.Status;
            }

            LogAudit("PatientTest", patientTestId, pt.IsReviewed ? "Review" : "Unreview");
            await _context.SaveChangesAsync();
        }

        public async Task ToggleEnteredAsync(int patientTestId)
        {
            var pt = await _context.PatientTests.FindAsync(patientTestId);
            if (pt == null) return;

            if (pt.Status == TestStatus.Entered)
            {
                pt.Status = TestStatus.New;
            }
            else if (pt.Status == TestStatus.New)
            {
                pt.Status = TestStatus.Entered;
                pt.EnteredByUserId = _currentUserService.CurrentUser?.Id;
                pt.EnteredAt = DateTime.Now;
            }

            LogAudit("PatientTest", patientTestId, pt.Status == TestStatus.Entered ? "Enter" : "Unenter");
            await _context.SaveChangesAsync();
        }

        public async Task TogglePrintedAsync(int patientTestId)
        {
            var pt = await _context.PatientTests.FindAsync(patientTestId);
            if (pt == null) return;

            pt.IsPrinted = !pt.IsPrinted;
            if (pt.IsPrinted)
            {
                pt.Status = TestStatus.Printed;
                pt.PrintedByUserId = _currentUserService.CurrentUser?.Id;
            }
            else
            {
                pt.Status = pt.Status == TestStatus.Printed ? TestStatus.Reviewed : pt.Status;
            }

            LogAudit("PatientTest", patientTestId, pt.IsPrinted ? "Print" : "Unprint");
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientNoteAsync(int patientId, string note)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return;

            patient.Notes = note;
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLog>> GetAuditForPatientAsync(int patientId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.EntityName == "Patient" && a.EntityId == patientId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetAuditForTestAsync(int patientTestId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.EntityName == "PatientTest" && a.EntityId == patientTestId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<PatientListItem>> SearchByNameAsync(string partialName, DateTime forDate)
        {
            var items = await _context.Patients
                .Where(p => p.FullName.Contains(partialName))
                .SelectMany(p => p.Visits.Where(v => v.VisitDate.Date == forDate.Date).DefaultIfEmpty(),
                    (patient, visit) => new { patient, visit })
                .Select(x => new PatientListItem(
                    x.patient.Id,
                    x.visit != null ? x.visit.Id : 0,
                    x.patient.FullName,
                    TestStatus.New,
                    0,
                    x.patient.IsImportant,
                    x.visit != null ? x.visit.DailySequenceNumber : 0,
                    x.patient.Gender,
                    x.patient.AgeValue,
                    x.patient.AgeUnit,
                    x.patient.LabId,
                    x.patient.FileCode,
                    x.patient.VisitCode,
                    x.patient.Notes))
                .ToListAsync();

            return items;
        }

        private void LogAudit(string entityName, int entityId, string action)
        {
            var audit = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                UserId = _currentUserService.CurrentUser?.Id ?? 0,
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(audit);
        }
    }
}
