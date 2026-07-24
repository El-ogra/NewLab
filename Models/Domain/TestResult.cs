using System;
using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class TestResult
    {
        public int Id { get; set; }

        public int PatientTestId { get; set; }

        public int LabTestElementId { get; set; }

        [MaxLength(100)]
        public string? Value { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        public bool IsAbnormal { get; set; }

        public bool IsCritical { get; set; }

        [MaxLength(100)]
        public string? FlagText { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime EnteredAt { get; set; } = DateTime.Now;

        public DateTime? ReviewedAt { get; set; }

        public bool IsReviewed { get; set; }

        public bool IsPrinted { get; set; }

        // Navigation properties
        public PatientTest PatientTest { get; set; } = null!;

        public LabTestElement Element { get; set; } = null!;
    }
}
