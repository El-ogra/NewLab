using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;

namespace NewLab.Services.Interfaces
{
    public sealed record DeliveryPatientRow(
        int PatientId,
        int VisitId,
        string FullName,
        TestStatus AggregateStatus,
        int TestCount,
        int UndeliveredCount,
        int UnprintedCount,
        decimal RemainingBalance,
        bool IsImportant,
        int AttendanceNumber);

    public sealed record DeliveryPatientTestRow(
        int PatientTestId,
        int LabTestId,
        string TestName,
        TestStatus Status,
        bool IsPrinted,
        bool IsDelivered,
        decimal Price);

    public sealed record DeliveryFilter(
        bool OnlyUndelivered = true,
        bool OnlyLabToLab = false,
        bool OnlyIndividual = false,
        bool OnlyImportant = false,
        DateTime? DateFrom = null,
        DateTime? DateTo = null);

    public interface IDeliveryService
    {
        Task<List<DeliveryPatientRow>> GetUndeliveredTodayAsync();
        Task<List<DeliveryPatientRow>> FilterAsync(DeliveryFilter filter);
        Task<List<DeliveryPatientTestRow>> GetPatientTestsAsync(int patientId);
        Task<(int Undelivered, int Unprinted, decimal Remaining)> GetPatientDeliveryStateAsync(int patientId);
        Task DeliverAllResultsAsync(int patientId, int userId);
        Task UnmarkDeliveredAsync(int patientId, int userId);
        Task<PaymentTransaction> SettleAccountAsync(int patientId, decimal amount, int userId, string? note = null);
        Task<DeliveryPatientRow?> SearchByCodeAsync(string code);
    }
}
