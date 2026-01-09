using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YC3_DAT_VE_CONCERT.Model
{
    [Table("venues")]
    public class Venue
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Venue name must be between 3 and 200 characters")]
        [Column("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(500)]
        [Column("location")]
        public string Location { get; set; }

        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000")]
        [Column("capacity")]
        public int Capacity { get; set; }

        // Navigation Properties
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
