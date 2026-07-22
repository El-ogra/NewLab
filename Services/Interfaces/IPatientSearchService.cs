using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;

namespace NewLab.Services.Interfaces
{
    public enum SearchSource
    {
        Main,
        Backup
    }

    public sealed record SearchCriteria(
        string? NamePrefix = null,
        string? NameContains = null,
        string? PhoneNumber = null,
        string? NationalId = null,
        string? VisitCode = null,
        string? LabCode = null,
        string? FileCode = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null,
        int? AgeFrom = null,
        int? AgeTo = null,
        AgeUnit? AgeUnit = null,
        Gender? Gender = null,
        int? ReferralId = null,
        SearchSource Source = SearchSource.Main,
        int MaxResults = 100);

    public sealed record PatientSearchRow(
        int PatientId,
        string FullName,
        Gender Gender,
        int AgeValue,
        AgeUnit AgeUnit,
        string? PhoneNumber,
        string? NationalId,
        string? LabId,
        string? FileCode,
        string? VisitCode,
        string? ReferralName,
        DateTime CreatedAt,
        bool IsImportant);

    public sealed record PatientTestsSummary(
        int TotalTests,
        int UnenteredCount,
        int UnprintedCount,
        int UndeliveredCount,
        decimal Total,
        decimal Paid,
        decimal Remaining);

    public interface IPatientSearchService
    {
        Task<List<PatientSearchRow>> SearchAsync(SearchCriteria criteria);
        Task<List<PatientTest>> GetPatientTestsSummaryAsync(int patientId);
        Task<PatientTestsSummary> GetSummaryAsync(int patientId);
        Task DeletePatientAsync(int patientId);
    }
}
