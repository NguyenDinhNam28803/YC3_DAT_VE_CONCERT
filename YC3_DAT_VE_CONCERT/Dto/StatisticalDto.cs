namespace YC3_DAT_VE_CONCERT.Dto
{
    public class StatisticalDto
    {
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
    }

    public class EventStatisticalResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int TotalSeat { get; set; }
        public decimal TicketPrice { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public int VenueCapacity { get; set; }
        public string Description { get; set; }
        public int TotalTicketsSold { get; set; }
        public int AvailableSeats { get; set; }
    }
}
