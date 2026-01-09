using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using YC3_DAT_VE_CONCERT.Validations;

namespace YC3_DAT_VE_CONCERT.Model
{
    [Table("events")]
    public class Event
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Event name must be between 3 and 200 characters")]
        [Column("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.DateTime)]
        [Column("date")]
        [FutureDate(ErrorMessage = "Event date must be in the future")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        [Column("venue_id")]
        public int VenueId { get; set; }

        [StringLength(1000)]
        [Column("description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Event ticket price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Event ticket price must be greater than 0")]
        [Column("ticket_price")]
        public decimal TicketPrice { get; set; }

        [Required(ErrorMessage = "Total seat is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total seat must be greater than 0")]
        [Column("total_seat")]
        public int TotalSeat { get; set; }

        // Navigation Properties
        [ForeignKey("VenueId")]
        public virtual Venue Venue { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
