namespace YC3_DAT_VE_CONCERT.Dto
{
    // Dto/TicketDto.cs
    public class TicketDto
    {
        public int TicketId { get; set; }
        public int OrderId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string QrCodeData { get; set; }
        public DateTime? PurchasedDate { get; set; }
    }

    // Dto/TicketPdfRequestDto.cs
    public class TicketPdfRequestDto
    {
        public int TicketId { get; set; }
        public bool IncludeQrCode { get; set; } = true;
        public bool IncludeLogo { get; set; } = true;
    }

    // Dto/BulkTicketPdfRequestDto.cs
    public class BulkTicketPdfRequestDto
    {
        public int OrderId { get; set; }
        public bool IncludeQrCode { get; set; } = true;
    }
}
