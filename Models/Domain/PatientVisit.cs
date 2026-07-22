using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class PatientVisit
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public DateTime VisitDate { get; set; }

        public int DailySequenceNumber { get; set; }

        [MaxLength(50)]
        public string FullVisitCode { get; set; } = string.Empty;

        // Navigation properties
        public Patient Patient { get; set; } = null!;

        public ICollection<PatientTest> PatientTests { get; set; } = new List<PatientTest>();
    }
}
