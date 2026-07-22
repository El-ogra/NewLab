using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class SpecimenType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string ArabicName { get; set; } = string.Empty;
    }
}
