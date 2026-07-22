using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class LabTestElement
    {
        public int Id { get; set; }

        public int ParentLabTestId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public int ArrangeNumber { get; set; }

        public bool IsMainTest { get; set; }

        public LabTest ParentLabTest { get; set; } = null!;
    }
}
