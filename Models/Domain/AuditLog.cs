using System;
using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int EntityId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [MaxLength(2000)]
        public string? Details { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
