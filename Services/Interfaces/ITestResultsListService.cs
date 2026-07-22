using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;

namespace NewLab.Services.Interfaces
{
    public sealed record PatientListItem(
        int PatientId,
        int VisitId,
        string FullName,
        TestStatus AggregateStatus,
        int VisitCount,
        bool IsImportant,
        int AttendanceNumber);

    public interface ITestResultsListService
    {
        Task<List<PatientListItem>> GetTodayPatientsAsync();
        Task<List<PatientListItem>> GetPatientsByFilterAsync(string filterMode, DateTime? forDate);
        Task<List<PatientTest>> GetPatientTestsAsync(int patientId, DateTime? forDate);
        Task<PatientListItem?> SearchByCodeAsync(string code);
        Task<PatientListItem?> SearchByAttendanceNumberAsync(int number, DateTime? forDate);
        Task ToggleReviewedAsync(int patientTestId);
        Task ToggleEnteredAsync(int patientTestId);
        Task TogglePrintedAsync(int patientTestId);
        Task UpdatePatientNoteAsync(int patientId, string note);
        Task<List<AuditLog>> GetAuditForPatientAsync(int patientId);
        Task<List<AuditLog>> GetAuditForTestAsync(int patientTestId);
    }
}
