using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Skills { get; set; }

        [Required]
        public string Availability { get; set; }

        public DateTime DateRegistered { get; set; } = DateTime.Now;
    }
}