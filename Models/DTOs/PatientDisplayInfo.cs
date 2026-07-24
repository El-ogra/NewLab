using NewLab.Models.Domain.Enums;

namespace NewLab.Models.DTOs
{
    public record PatientDisplayInfo(
        int PatientId,
        string FullName,
        bool IsImportant,
        string? LabId,
        string? FileCode,
        string? VisitCode,
        Gender Gender,
        decimal AgeValue,
        AgeUnit AgeUnit);
}
