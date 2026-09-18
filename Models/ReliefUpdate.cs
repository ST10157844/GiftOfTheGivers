using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models
{
    public class ReliefUpdate
    {
        [Key]
        public int ReliefUpdateId { get; set; }

        [Required]
        public string ProjectTitle { get; set; }

        [Required]
        public string UpdateDetails { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}