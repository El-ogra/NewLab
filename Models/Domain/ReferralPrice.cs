using System.ComponentModel.DataAnnotations;

namespace NewLab.Models.Domain
{
    public class ReferralPrice
    {
        public int Id { get; set; }

        public int LabTestId { get; set; }

        public int ReferralId { get; set; }

        [Required]
        public decimal Price { get; set; }

        public LabTest LabTest { get; set; } = null!;

        public Referral Referral { get; set; } = null!;
    }
}
