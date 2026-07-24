using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLab.Models.Domain
{
    public class LabTest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string TestName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ReportNameLarge { get; set; }

        [MaxLength(200)]
        public string? ReportNameSmall { get; set; }

        [MaxLength(200)]
        public string? BillNameLarge { get; set; }

        [MaxLength(200)]
        public string? BillNameSmall { get; set; }

        [MaxLength(100)]
        public string? HistoryName { get; set; }

        [MaxLength(200)]
        public string? ArabicName { get; set; }

        public int? TestGroupId { get; set; }

        [MaxLength(100)]
        public string? LogGroup { get; set; }

        [MaxLength(500)]
        public string? Collection { get; set; }

        public int TestTimeDays { get; set; }

        public int ArrangeNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PatientPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LabToLabPrice { get; set; }

        public bool IsRoutine { get; set; }

        public bool IsSeeReport { get; set; }

        public bool IsPrintWithOther { get; set; }

        public bool IsAddWithGroup { get; set; }

        public bool IsMainTest { get; set; }

        public int? ParentLabTestId { get; set; }

        public int? DefaultSpecimenTypeId { get; set; }

        public bool IsSentExternal { get; set; }

        public int? ExternalReferralId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ExternalCost { get; set; }

        [MaxLength(500)]
        public string? PromptQuestion { get; set; }

        [MaxLength(100)]
        public string? LabelName { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public TestGroup? TestGroup { get; set; }

        public LabTest? ParentLabTest { get; set; }

        public SpecimenType? DefaultSpecimenType { get; set; }

        public Referral? ExternalReferral { get; set; }

        public ICollection<LabTestElement> Elements { get; set; } = new List<LabTestElement>();

        public ICollection<ReferralPrice> ReferralPrices { get; set; } = new List<ReferralPrice>();

        public ICollection<LabTestSpecimen> Specimens { get; set; } = new List<LabTestSpecimen>();
    }
}
