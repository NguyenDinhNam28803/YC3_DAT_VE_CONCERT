using System.ComponentModel.DataAnnotations;

namespace YC3_DAT_VE_CONCERT.Dto
{
    public class CreateOrderDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one ticket is required")]
        public List<CreateTicketInOrderDto> Tickets { get; set; }
    }

    public class CreateTicketInOrderDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int EventId { get; set; }

        [Required]
        [StringLength(10)]
        [RegularExpression("^[A-Z][0-9]{1,3}$")]
        public string SeatNumber { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        [RegularExpression("^(pending|completed|cancelled)$")]
        public string Status { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public List<TicketInOrderDto> Tickets { get; set; }
    }

    public class TicketInOrderDto
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string SeatNumber { get; set; }
    }
}
