using System;
using System.ComponentModel.DataAnnotations;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Domain
{
    public class PatientCode
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public CodeType CodeType { get; set; }

        [Required]
        [MaxLength(50)]
        public string CodeValue { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; }

        // Navigation property
        public Patient Patient { get; set; } = null!;
    }
}
