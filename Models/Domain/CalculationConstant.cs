using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLab.Models.Domain
{
    public class CalculationConstant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string TestType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ConstantName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,6)")]
        public decimal ConstantValue { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
