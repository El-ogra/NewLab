using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Domain
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Title { get; set; }

        public Gender Gender { get; set; }

        public int AgeValue { get; set; }

        public AgeUnit AgeUnit { get; set; }

        public BillingSystem BillingSystem { get; set; }

        public bool IsImportant { get; set; }

        public int? ReferralId { get; set; }

        public bool ReferralHiddenOnReport { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(20)]
        public string? NationalId { get; set; }

        [MaxLength(50)]
        public string? LabId { get; set; }

        [MaxLength(50)]
        public string? FileCode { get; set; }

        [MaxLength(50)]
        public string? VisitCode { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? ExternalSpecimenTypeId { get; set; }

        public decimal DiscountValue { get; set; }

        public bool DiscountIsPercent { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Boolean medical fields (Decision 1)
        public bool IsFasting { get; set; }

        public int? FastingHours { get; set; }

        public bool IsOnAnticoagulant { get; set; }

        public bool HasLiverTreatment { get; set; }

        public bool HasAntiviralTreatment { get; set; }

        public bool HasAntibiotic { get; set; }

        public bool IsPregnant { get; set; }

        public bool IsSmoker { get; set; }

        // Navigation properties
        public Referral? Referral { get; set; }

        public SpecimenType? ExternalSpecimenType { get; set; }

        public User CreatedByUser { get; set; } = null!;

        public ICollection<PatientVisit> Visits { get; set; } = new List<PatientVisit>();
    }
}
