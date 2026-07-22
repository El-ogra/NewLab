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
    public class PatientSearchService : IPatientSearchService
    {
        private readonly NewLabDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public PatientSearchService(NewLabDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<PatientSearchRow>> SearchAsync(SearchCriteria criteria)
        {
            if (criteria.Source == SearchSource.Backup)
                throw new NotImplementedException("هذه الميزة غير مفعّلة في هذه المرحلة");

            var q = _context.Patients.Include(p => p.Referral).AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.NamePrefix))
                q = q.Where(p => p.FullName.StartsWith(criteria.NamePrefix));

            if (!string.IsNullOrWhiteSpace(criteria.NameContains))
                q = q.Where(p => p.FullName.Contains(criteria.NameContains));

            if (!string.IsNullOrWhiteSpace(criteria.PhoneNumber))
                q = q.Where(p => p.PhoneNumber == criteria.PhoneNumber);

            if (!string.IsNullOrWhiteSpace(criteria.NationalId))
                q = q.Where(p => p.NationalId == criteria.NationalId);

            if (!string.IsNullOrWhiteSpace(criteria.VisitCode))
                q = q.Where(p => p.VisitCode == criteria.VisitCode);

            if (!string.IsNullOrWhiteSpace(criteria.LabCode))
                q = q.Where(p => p.LabId == criteria.LabCode);

            if (!string.IsNullOrWhiteSpace(criteria.FileCode))
                q = q.Where(p => p.FileCode == criteria.FileCode);

            if (criteria.DateFrom.HasValue)
                q = q.Where(p => p.CreatedAt >= criteria.DateFrom.Value);

            if (criteria.DateTo.HasValue)
                q = q.Where(p => p.CreatedAt < criteria.DateTo.Value.AddDays(1));

            if (criteria.Gender.HasValue)
                q = q.Where(p => p.Gender == criteria.Gender.Value);

            if (criteria.ReferralId.HasValue)
                q = q.Where(p => p.ReferralId == criteria.ReferralId.Value);

            if (criteria.AgeFrom.HasValue && criteria.AgeUnit.HasValue)
                q = q.Where(p => p.AgeUnit == criteria.AgeUnit.Value && p.AgeValue >= criteria.AgeFrom.Value);

            if (criteria.AgeTo.HasValue && criteria.AgeUnit.HasValue)
                q = q.Where(p => p.AgeUnit == criteria.AgeUnit.Value && p.AgeValue <= criteria.AgeTo.Value);

            return await q
                .OrderByDescending(p => p.CreatedAt)
                .Take(criteria.MaxResults)
                .Select(p => new PatientSearchRow(
                    p.Id, p.FullName, p.Gender, p.AgeValue, p.AgeUnit,
                    p.PhoneNumber, p.NationalId, p.LabId, p.FileCode, p.VisitCode,
                    p.Referral != null ? p.Referral.Name : null,
                    p.CreatedAt, p.IsImportant))
                .ToListAsync();
        }

        public async Task<List<PatientTest>> GetPatientTestsSummaryAsync(int patientId)
        {
            return await _context.PatientTests
                .Include(pt => pt.Visit)
                .Include(pt => pt.LabTest)
                .Where(pt => pt.Visit.PatientId == patientId)
                .OrderByDescending(pt => pt.Visit.VisitDate)
                .ToListAsync();
        }

        public async Task<PatientTestsSummary> GetSummaryAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
                return new PatientTestsSummary(0, 0, 0, 0, 0, 0, 0);

            var tests = await _context.PatientTests
                .Include(pt => pt.Visit)
                .Where(pt => pt.Visit.PatientId == patientId)
                .ToListAsync();

            return new PatientTestsSummary(
                tests.Count,
                tests.Count(t => t.Status == TestStatus.New),
                tests.Count(t => !t.IsPrinted),
                tests.Count(t => !t.IsDelivered),
                patient.TotalAmount,
                patient.PaidAmount,
                patient.TotalAmount - patient.PaidAmount);
        }

        public async Task DeletePatientAsync(int patientId)
        {
            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedAccessException("حذف المريض يتطلّب صلاحية Admin");

            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return;

            _context.Patients.Remove(patient);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Patient",
                EntityId = patientId,
                Action = "Delete",
                UserId = _currentUserService.CurrentUser!.Id,
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}
