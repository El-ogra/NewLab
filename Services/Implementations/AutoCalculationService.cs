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
    public class AutoCalculationService : IAutoCalculationService
    {
        private readonly NewLabDbContext _context;

        public AutoCalculationService(NewLabDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> CalculateHctAsync(decimal hgb)
        {
            var constant = await GetConstantAsync("CBC", "HctMultiplier");
            return hgb * (constant?.ConstantValue ?? 3.3m);
        }

        public async Task<decimal> CalculateHgbPercentAsync(decimal hgb, int ageYears, Gender gender)
        {
            string constantName = gender switch
            {
                Gender.Female when ageYears > 12 => "FemaleOver12",
                Gender.Male when ageYears > 12 => "MaleOver12",
                _ when ageYears <= 1 => "AgeUnder1",
                _ => "Age1To12"
            };

            var constant = await GetConstantAsync("Hgb", constantName);
            var divisor = constant?.ConstantValue ?? 7.5m;
            return Math.Round(hgb / divisor * 100, 1);
        }

        public async Task<decimal> CalculateINRAsync(decimal ptPatient, decimal? overrideControlTime = null, decimal? overrideIsi = null)
        {
            var controlTime = overrideControlTime ?? (await GetConstantAsync("PT", "ControlTime"))?.ConstantValue ?? 12.0m;
            var isi = overrideIsi ?? (await GetConstantAsync("PT", "ISI"))?.ConstantValue ?? 1.0m;

            if (controlTime == 0) return 0;

            return (decimal)Math.Pow((double)(ptPatient / controlTime), (double)isi);
        }

        public async Task<decimal> CalculatePTTRatioAsync(decimal pttPatient, decimal? overrideControlTime = null)
        {
            var controlTime = overrideControlTime ?? (await GetConstantAsync("PTT", "ControlTime"))?.ConstantValue ?? 30.0m;

            if (controlTime == 0) return 0;

            return pttPatient / controlTime;
        }

        public async Task<List<CalculationConstant>> GetConstantsAsync(string? testType = null)
        {
            var query = _context.CalculationConstants.AsQueryable();
            if (!string.IsNullOrEmpty(testType))
                query = query.Where(cc => cc.TestType == testType);
            return await query.OrderBy(cc => cc.TestType).ThenBy(cc => cc.ConstantName).ToListAsync();
        }

        public async Task UpdateConstantAsync(int id, decimal value)
        {
            var constant = await _context.CalculationConstants.FindAsync(id);
            if (constant != null)
            {
                constant.ConstantValue = value;
                constant.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<CalculationConstant?> GetConstantAsync(string testType, string constantName)
        {
            return await _context.CalculationConstants
                .FirstOrDefaultAsync(cc => cc.TestType == testType && cc.ConstantName == constantName);
        }
    }
}
