using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Domain
{
    public class PatientTest
    {
        public int Id { get; set; }

        public int PatientVisitId { get; set; }

        public int LabTestId { get; set; }

        public TestStatus Status { get; set; } = TestStatus.New;

        public bool IsReviewed { get; set; }

        public bool IsPrinted { get; set; }

        public bool IsDelivered { get; set; }

        public int? EnteredByUserId { get; set; }

        public DateTime? EnteredAt { get; set; }

        public int? ReviewedByUserId { get; set; }

        public int? PrintedByUserId { get; set; }

        public int? DeliveredByUserId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation properties
        public PatientVisit Visit { get; set; } = null!;

        public LabTest LabTest { get; set; } = null!;

        public User? EnteredByUser { get; set; }

        public User? ReviewedByUser { get; set; }

        public User? PrintedByUser { get; set; }

        public User? DeliveredByUser { get; set; }
    }
}
