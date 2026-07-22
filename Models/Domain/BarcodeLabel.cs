using System.Collections.Generic;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Domain
{
    public class BarcodeLabel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int? SpecimenTypeId { get; set; }
        public string SpecimenName { get; set; } = string.Empty;
        public List<string> Tests { get; set; } = new();
        public string Code { get; set; } = string.Empty;
        public CodeType Type { get; set; }
    }
}
