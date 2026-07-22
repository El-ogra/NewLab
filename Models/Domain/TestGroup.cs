using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class TestGroup
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LogGroup { get; set; }

        public ICollection<LabTest> Tests { get; set; } = new List<LabTest>();
    }
}
