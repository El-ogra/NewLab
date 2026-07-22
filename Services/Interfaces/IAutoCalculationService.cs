using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;

namespace NewLab.Services.Interfaces
{
    public interface IAutoCalculationService
    {
        Task<decimal> CalculateHctAsync(decimal hgb);
        Task<decimal> CalculateHgbPercentAsync(decimal hgb, int ageYears, Gender gender);
        Task<decimal> CalculateINRAsync(decimal ptPatient, decimal? overrideControlTime = null, decimal? overrideIsi = null);
        Task<decimal> CalculatePTTRatioAsync(decimal pttPatient, decimal? overrideControlTime = null);
        Task<List<CalculationConstant>> GetConstantsAsync(string? testType = null);
        Task UpdateConstantAsync(int id, decimal value);
    }
}
