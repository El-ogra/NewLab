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
    public class NormalRangeService : INormalRangeService
    {
        private readonly NewLabDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public NormalRangeService(NewLabDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<NormalRange>> GetForTestAsync(int labTestId)
        {
            return await _context.NormalRanges
                .Where(nr => nr.LabTestId == labTestId)
                .OrderBy(nr => nr.Gender)
                .ThenBy(nr => nr.AgeFrom)
                .ToListAsync();
        }

        public async Task<NormalRange> AddAsync(NormalRange range)
        {
            _context.NormalRanges.Add(range);
            await _context.SaveChangesAsync();
            return range;
        }

        public async Task<NormalRange> UpdateAsync(NormalRange range)
        {
            _context.NormalRanges.Update(range);
            await _context.SaveChangesAsync();
            return range;
        }

        public async Task DeleteAsync(int rangeId)
        {
            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedAccessException("عملية الحذف تتطلب صلاحية Admin");
            }

            var range = await _context.NormalRanges.FindAsync(rangeId);
            if (range != null)
            {
                _context.NormalRanges.Remove(range);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<NormalRange?> GetMatchingRangeAsync(int labTestId, Patient patient)
        {
            // Decision 16: Narrowest range wins
            var matchingRanges = await _context.NormalRanges
                .Where(nr => nr.LabTestId == labTestId)
                .Where(nr => nr.Gender == patient.Gender)
                .ToListAsync();

            var matched = new List<NormalRange>();
            foreach (var nr in matchingRanges)
            {
                var patientAge = ConvertAgeToUnit(patient.AgeValue, patient.AgeUnit, nr.AgeUnit);
                if (patientAge >= nr.AgeFrom && patientAge <= nr.AgeTo)
                {
                    matched.Add(nr);
                }
            }

            if (!matched.Any())
                return null;

            // Decision 16: narrowest range wins (smallest AgeTo - AgeFrom)
            return matched.OrderBy(nr => nr.AgeTo - nr.AgeFrom).First();
        }

        public Task<NormalRangeEvaluation> EvaluateValueAsync(NormalRange range, decimal value)
        {
            // Check critical low first
            if (range.CriticalLowLimit.HasValue && value < range.CriticalLowLimit.Value)
                return Task.FromResult(new NormalRangeEvaluation("CriticalLow", range.CriticalFlag));

            // Check critical high
            if (range.CriticalHighLimit.HasValue && value > range.CriticalHighLimit.Value)
                return Task.FromResult(new NormalRangeEvaluation("CriticalHigh", range.CriticalFlag));

            // Check abnormal low
            if (value < range.LowLimit)
                return Task.FromResult(new NormalRangeEvaluation("AbnormalLow", range.LowFlag));

            // Check abnormal high
            if (value > range.HighLimit)
                return Task.FromResult(new NormalRangeEvaluation("AbnormalHigh", range.HighFlag));

            // Normal
            return Task.FromResult(new NormalRangeEvaluation("Normal", null));
        }

        private static decimal ConvertAgeToUnit(int ageValue, AgeUnit fromUnit, AgeUnit toUnit)
        {
            if (fromUnit == toUnit)
                return ageValue;

            // Convert everything to days first, then to target
            decimal days = fromUnit switch
            {
                AgeUnit.Day => ageValue,
                AgeUnit.Month => ageValue * 30m,
                AgeUnit.Year => ageValue * 365m,
                _ => ageValue
            };

            return toUnit switch
            {
                AgeUnit.Day => days,
                AgeUnit.Month => days / 30m,
                AgeUnit.Year => days / 365m,
                _ => days
            };
        }
    }
}
