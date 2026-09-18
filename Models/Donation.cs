using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }

        [Required]
        public string DonorName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public string DonationType { get; set; }

        public DateTime DonationDate { get; set; } = DateTime.Now;
    }
}