using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class SavedComment
    {
        public int Id { get; set; }

        public int LabTestId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string CommentText { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        // Navigation property
        public LabTest LabTest { get; set; } = null!;
    }
}
