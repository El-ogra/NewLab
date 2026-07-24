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
    public class DeliveryService : IDeliveryService
    {
        private readonly NewLabDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeliveryService(NewLabDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<DeliveryPatientRow>> GetUndeliveredTodayAsync()
        {
            return await FilterAsync(new DeliveryFilter(OnlyUndelivered: true, DateFrom: DateTime.Today, DateTo: DateTime.Today));
        }

        public async Task<List<DeliveryPatientRow>> FilterAsync(DeliveryFilter filter)
        {
            var dateFrom = filter.DateFrom ?? DateTime.Today;
            var dateTo = filter.DateTo ?? DateTime.Today;

            var visits = await _context.PatientVisits
                .Include(v => v.Patient)
                .Include(v => v.PatientTests)
                .ThenInclude(pt => pt.LabTest)
                .Where(v => v.VisitDate.Date >= dateFrom.Date && v.VisitDate.Date <= dateTo.Date)
                .ToListAsync();

            var result = new List<DeliveryPatientRow>();

            foreach (var visit in visits)
            {
                var tests = visit.PatientTests?.ToList() ?? new List<PatientTest>();
                if (!tests.Any()) continue;

                var undeliveredCount = tests.Count(t => !t.IsDelivered);
                var unprintedCount = tests.Count(t => !t.IsPrinted);

                if (filter.OnlyUndelivered && undeliveredCount == 0) continue;

                var aggregateStatus = tests.Any(t => t.Status == TestStatus.New)
                    ? TestStatus.New
                    : tests.Min(t => t.Status);

                var remainingBalance = visit.Patient.TotalAmount - visit.Patient.PaidAmount;

                if (filter.OnlyImportant && !visit.Patient.IsImportant) continue;
                if (filter.OnlyIndividual && visit.Patient.BillingSystem != BillingSystem.Individual) continue;
                if (filter.OnlyLabToLab && visit.Patient.BillingSystem != BillingSystem.LabToLab) continue;
                if (filter.UserId.HasValue && visit.Patient.CreatedByUserId != filter.UserId.Value) continue;

                result.Add(new DeliveryPatientRow(
                    visit.Patient.Id,
                    visit.Id,
                    visit.Patient.FullName,
                    aggregateStatus,
                    tests.Count,
                    undeliveredCount,
                    unprintedCount,
                    remainingBalance,
                    visit.Patient.IsImportant,
                    visit.DailySequenceNumber,
                    visit.Patient.Gender,
                    visit.Patient.AgeValue,
                    visit.Patient.AgeUnit,
                    visit.Patient.LabId,
                    visit.Patient.FileCode,
                    visit.Patient.VisitCode));
            }

            return result.OrderBy(p => p.AttendanceNumber).ToList();
        }

        public async Task<List<DeliveryPatientTestRow>> GetPatientTestsAsync(int patientId)
        {
            var tests = await _context.PatientTests
                .Include(pt => pt.Visit)
                .Include(pt => pt.LabTest)
                .Where(pt => pt.Visit.PatientId == patientId)
                .OrderByDescending(pt => pt.Visit.VisitDate)
                .ToListAsync();

            return tests.Select(pt => new DeliveryPatientTestRow(
                pt.Id,
                pt.LabTestId,
                pt.LabTest.TestName,
                pt.Status,
                pt.IsReviewed,
                pt.Status >= TestStatus.Entered,
                pt.IsPrinted,
                pt.IsDelivered,
                pt.Price)).ToList();
        }

        public async Task<(int Unentered, int Undelivered, int Unprinted, decimal Remaining)> GetPatientDeliveryStateAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return (0, 0, 0, 0);

            var tests = await _context.PatientTests
                .Include(pt => pt.Visit)
                .Where(pt => pt.Visit.PatientId == patientId)
                .ToListAsync();

            var unentered = tests.Count(t => t.Status == TestStatus.New);
            var undelivered = tests.Count(t => !t.IsDelivered);
            var unprinted = tests.Count(t => !t.IsPrinted);
            var remaining = patient.TotalAmount - patient.PaidAmount;

            return (unentered, undelivered, unprinted, remaining);
        }

        public async Task DeliverAllResultsAsync(int patientId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var tests = await _context.PatientTests
                    .Include(pt => pt.Visit)
                    .Where(pt => pt.Visit.PatientId == patientId)
                    .ToListAsync();

                foreach (var test in tests)
                {
                    test.IsDelivered = true;
                    test.DeliveredByUserId = userId;
                    if (test.Status >= TestStatus.Printed)
                        test.Status = TestStatus.Delivered;
                }

                var patient = await _context.Patients.FindAsync(patientId);
                if (patient != null)
                {
                    patient.DeliveredAt = DateTime.Now;
                    patient.DeliveredByUserId = userId;
                }

                _context.PaymentTransactions.Add(new PaymentTransaction
                {
                    PatientId = patientId,
                    Amount = 0,
                    Type = PaymentType.Delivery,
                    UserId = userId,
                    Timestamp = DateTime.Now,
                    Note = "تسجيل عملية تسليم"
                });

                _context.AuditLogs.Add(new AuditLog
                {
                    EntityName = "Patient",
                    EntityId = patientId,
                    Action = "Deliver",
                    UserId = userId,
                    Timestamp = DateTime.Now,
                    Details = "تسليم جميع النتائج"
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UnmarkDeliveredAsync(int patientId, int userId)
        {
            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedAccessException("عملية إلغاء التسليم تتطلّب صلاحية Admin");

            var tests = await _context.PatientTests
                .Include(pt => pt.Visit)
                .Where(pt => pt.Visit.PatientId == patientId)
                .ToListAsync();

            foreach (var test in tests)
            {
                test.IsDelivered = false;
                test.DeliveredByUserId = null;
            }

            var patient = await _context.Patients.FindAsync(patientId);
            if (patient != null)
            {
                patient.DeliveredAt = null;
                patient.DeliveredByUserId = null;
            }

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Patient",
                EntityId = patientId,
                Action = "UnmarkDelivered",
                UserId = userId,
                Timestamp = DateTime.Now,
                Details = "إلغاء التسليم"
            });

            await _context.SaveChangesAsync();
        }

        public async Task<PaymentTransaction> SettleAccountAsync(int patientId, decimal amount, int userId, string? note = null)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
                throw new InvalidOperationException("المريض غير موجود");

            patient.PaidAmount += amount;

            var tx = new PaymentTransaction
            {
                PatientId = patientId,
                Amount = amount,
                Type = PaymentType.Payment,
                UserId = userId,
                Timestamp = DateTime.Now,
                Note = note
            };

            _context.PaymentTransactions.Add(tx);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Patient",
                EntityId = patientId,
                Action = "SettleAccount",
                UserId = userId,
                Timestamp = DateTime.Now,
                Details = $"تسديد مبلغ {amount}"
            });

            await _context.SaveChangesAsync();
            return tx;
        }

        public async Task ClearAccountAsync(int patientId, int userId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
                throw new InvalidOperationException("المريض غير موجود");

            var remaining = patient.TotalAmount - patient.PaidAmount;
            if (remaining <= 0)
                throw new InvalidOperationException("لا يوجد متبقي للتحصيل");

            patient.PaidAmount = patient.TotalAmount;

            _context.PaymentTransactions.Add(new PaymentTransaction
            {
                PatientId = patientId,
                Amount = remaining,
                Type = PaymentType.Payment,
                UserId = userId,
                Timestamp = DateTime.Now,
                Note = "تحصيل كامل الحساب"
            });

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Patient",
                EntityId = patientId,
                Action = "ClearAccount",
                UserId = userId,
                Timestamp = DateTime.Now,
                Details = $"تحصيل مبلغ {remaining}"
            });

            await _context.SaveChangesAsync();
        }

        public async Task<DeliveryPatientRow?> SearchByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var trimmed = code.Trim();

            if (trimmed.Length >= 1)
            {
                var typeDigit = trimmed[0];
                var result = typeDigit switch
                {
                    '1' => await SearchByVisitCodeAsync(trimmed),
                    '3' => await SearchByFileCodeAsync(trimmed),
                    '5' => await SearchByLabIdAsync(trimmed),
                    _ => await SearchByFallbackAsync(trimmed)
                };
                return result;
            }

            return await SearchByFallbackAsync(trimmed);
        }

        private async Task<DeliveryPatientRow?> SearchByVisitCodeAsync(string code)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.VisitCode == code);
            return await BuildDeliveryRowAsync(patient);
        }

        private async Task<DeliveryPatientRow?> SearchByFileCodeAsync(string code)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FileCode == code);
            return await BuildDeliveryRowAsync(patient);
        }

        private async Task<DeliveryPatientRow?> SearchByLabIdAsync(string code)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.LabId == code);
            return await BuildDeliveryRowAsync(patient);
        }

        private async Task<DeliveryPatientRow?> SearchByFallbackAsync(string code)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p =>
                p.LabId == code || p.FileCode == code || p.VisitCode == code);
            return await BuildDeliveryRowAsync(patient);
        }

        private async Task<DeliveryPatientRow?> BuildDeliveryRowAsync(Patient? patient)
        {
            if (patient == null) return null;

            var visit = await _context.PatientVisits
                .Include(v => v.PatientTests)
                .ThenInclude(pt => pt.LabTest)
                .Where(v => v.PatientId == patient.Id)
                .OrderByDescending(v => v.VisitDate)
                .FirstOrDefaultAsync();

            if (visit == null) return null;

            var tests = visit.PatientTests?.ToList() ?? new List<PatientTest>();
            if (!tests.Any()) return null;

            var undeliveredCount = tests.Count(t => !t.IsDelivered);
            var unprintedCount = tests.Count(t => !t.IsPrinted);

            var aggregateStatus = tests.Any(t => t.Status == TestStatus.New)
                ? TestStatus.New
                : tests.Min(t => t.Status);

            var remainingBalance = patient.TotalAmount - patient.PaidAmount;

            return new DeliveryPatientRow(
                patient.Id,
                visit.Id,
                patient.FullName,
                aggregateStatus,
                tests.Count,
                undeliveredCount,
                unprintedCount,
                remainingBalance,
                patient.IsImportant,
                visit.DailySequenceNumber,
                patient.Gender,
                patient.AgeValue,
                patient.AgeUnit,
                patient.LabId,
                patient.FileCode,
                patient.VisitCode);
        }
    }
}
