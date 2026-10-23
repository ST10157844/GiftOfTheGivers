//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace GiftOfTheGivers.Models
//{
//    public class Donation
//    {
//        [Key]
//        public int DonationId { get; set; }

//        [Required]
//        public string DonorName { get; set; } = string.Empty;

//        [Required]
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal Amount { get; set; }

//        [Required]
//        public string Currency { get; set; } = string.Empty;

//        [Required]
//        public string DonationType { get; set; } = string.Empty;

//        public DateTime DonationDate { get; set; } = DateTime.Now;
//    }
//}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Donation : IValidatableObject
    {
        [Key]
        public int DonationId { get; set; }

        public string? DonorName { get; set; }

        [Range(typeof(decimal), "1", "79228162514264337593543950335",
            ErrorMessage = "Donation amount must be greater than zero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = string.Empty;

        [Required]
        public string DonationType { get; set; } = string.Empty;

        public bool IsAnonymous { get; set; }

        public string? TaxCertificateNumber { get; set; }

        public DateTime DonationDate { get; set; } = DateTime.Now;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (!IsAnonymous &&
                string.IsNullOrWhiteSpace(DonorName))
            {
                yield return new ValidationResult(
                    "Donor name is required unless the donation is anonymous.",
                    new[] { nameof(DonorName) });
            }

            string[] currencies = { "ZAR", "USD", "EUR" };

            if (!currencies.Contains(Currency))
            {
                yield return new ValidationResult(
                    "Currency must be ZAR, USD, or EUR.",
                    new[] { nameof(Currency) });
            }

            string[] donationTypes =
            {
                "One-Time",
                "Recurring"
            };

            if (!donationTypes.Contains(DonationType))
            {
                yield return new ValidationResult(
                    "Donation type must be One-Time or Recurring.",
                    new[] { nameof(DonationType) });
            }
        }
    }
}