namespace YC3_DAT_VE_CONCERT.Dto
{
    // kiểu trả về Ticket cho Admin
    public class TicketDtoResponse
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string SeatNumber { get; set; }
        public DateTime EventDate { get; set; }
        public decimal Price { get; set; }
    }

    // Kiểu trả về Ticket cho User
    public class TicketUserDtoResponse
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string UserName { get; set; }
        public DateTime EventDate { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
    }

    // thêm vé có sẵn
    public class TicketDtoRequest
    {
        public int EventId { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
    }

    // Kiểu đặt vé cho User
    public class TicketUserDtoRequest
    {
        public int TicketId { get; set; }
        public string SeatNumber { get; set; }
    }

    // cập nhật vé
    public class UpdateTicketDto
    {
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
    }
}
