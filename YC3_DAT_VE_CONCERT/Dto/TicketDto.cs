namespace YC3_DAT_VE_CONCERT.Dto
{
    public class TicketDtoResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime? PurchaseDate { get; set; }
    }

    public class TicketDtoRequest
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateTicketDto
    {
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
    }
}
