using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Domain
{
    public class NormalRange
    {
        public int Id { get; set; }

        public int LabTestId { get; set; }

        [Required]
        [MaxLength(200)]
        public string TestName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? TestUnit { get; set; }

        public Gender Gender { get; set; }

        public int AgeFrom { get; set; }

        public int AgeTo { get; set; }

        public AgeUnit AgeUnit { get; set; }

        [MaxLength(200)]
        public string? NormalRangeText { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal LowLimit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal HighLimit { get; set; }

        [MaxLength(50)]
        public string? LowFlag { get; set; }

        [MaxLength(50)]
        public string? HighFlag { get; set; }

        [MaxLength(500)]
        public string? LowComment { get; set; }

        [MaxLength(500)]
        public string? HighComment { get; set; }

        [MaxLength(500)]
        public string? CriticalComment { get; set; }

        [MaxLength(200)]
        public string? CriticalRangeText { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CriticalLowLimit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CriticalHighLimit { get; set; }

        [MaxLength(50)]
        public string? CriticalFlag { get; set; }

        // Navigation property
        public LabTest LabTest { get; set; } = null!;
    }
}
