using System;
using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class Referral
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public decimal DiscountPercent { get; set; }

        public bool IsDefaultLab { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
