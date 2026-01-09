using System.ComponentModel.DataAnnotations;
using YC3_DAT_VE_CONCERT.Validations;

namespace YC3_DAT_VE_CONCERT.Dto
{
    public class CreateEventDto
    {
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Event date must be in the future")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid venue")]
        public int VenueId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Total seat is required")]
        public int TotalSeat { get; set; }
    }

    public class UpdateEventDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [FutureDate]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VenueId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int TotalSeat { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public int VenueCapacity { get; set; }
        public string? Description { get; set; }
        public int TotalTicketsSold { get; set; }
        public int AvailableSeats { get; set; }
    }
}
