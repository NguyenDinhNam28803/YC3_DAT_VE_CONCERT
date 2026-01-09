using System.ComponentModel.DataAnnotations;

namespace YC3_DAT_VE_CONCERT.Dto
{
    public class CreateVenueDto
    {
        [Required(ErrorMessage = "Venue name is required")]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(500)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000")]
        public int Capacity { get; set; }
    }

    public class UpdateVenueDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Location { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Capacity { get; set; }
    }

    public class VenueResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int TotalEvents { get; set; }
        public int UpcomingEvents { get; set; }
    }
}
