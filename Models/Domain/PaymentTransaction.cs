using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Domain
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentType Type { get; set; }

        public int UserId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Note { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
