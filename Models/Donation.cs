using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }

        [Required]
        public string DonorName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = string.Empty;

        [Required]
        public string DonationType { get; set; } = string.Empty;

        public DateTime DonationDate { get; set; } = DateTime.Now;
    }
}