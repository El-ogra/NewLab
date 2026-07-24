using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class LabTestSpecimen
    {
        public int Id { get; set; }

        public int LabTestId { get; set; }

        public int SpecimenTypeId { get; set; }

        public int TubeOrder { get; set; } = 1;

        [MaxLength(100)]
        public string? LabelName { get; set; }

        // Navigation properties
        public LabTest LabTest { get; set; } = null!;

        public SpecimenType SpecimenType { get; set; } = null!;
    }
}
