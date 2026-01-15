using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class StatisticalService : IStatisticalService
    {
        private readonly ApplicationDbContext _context;
        public StatisticalService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy dữ liệu thống kê
        public async Task<StatisticalDto> GetStatisticalData()
        {
            try
            {
                var ticketSold = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Sold);
                var totalRevenue = await _context.Tickets.Where(t => t.Status == TicketStatus.Sold).SumAsync(t => t.Price);
                var totalCustomers = await _context.Customers.CountAsync();
                var statisticalData = new StatisticalDto
                {
                    TotalTicketsSold = ticketSold,
                    TotalRevenue = totalRevenue,
                    TotalCustomers = totalCustomers
                };
                return statisticalData;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving statistical data.", ex);
            }
        }

        // Lấy danh sách sự kiện
        public async Task<List<EventStatisticalResponseDto>> GetEventList()
        {
            try
            {
                var events = await _context.Events
                    .Select(e => new EventStatisticalResponseDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Date = e.Date,
                        TotalSeat = e.TotalSeat,
                        TicketPrice = e.TicketPrice,
                        VenueName = e.Venue.Name,
                        VenueLocation = e.Venue.Location,
                        VenueCapacity = e.Venue.Capacity,
                        Description = e.Description ?? "",
                        TotalTicketsSold = e.Tickets.Count(t => t.Status == TicketStatus.Sold),
                        AvailableSeats = e.TotalSeat - e.Tickets.Count(t => t.Status == TicketStatus.Sold)
                    })
                    .ToListAsync();
                return events;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving event list.", ex);
            }
        }

        public async Task<EventStatisticalResponseDto> GetEventById(int eventId)
        {
            try
            {
                var eventDetails = await _context.Events
                    .Where(ev => ev.Id == eventId)
                    .Select(ev => new EventStatisticalResponseDto
                    {
                        Id = ev.Id,
                        Name = ev.Name,
                        Date = ev.Date,
                        TotalSeat = ev.TotalSeat,
                        VenueName = ev.Venue.Name,
                        VenueLocation = ev.Venue.Location,
                        VenueCapacity = ev.Venue.Capacity,
                        Description = ev.Description ?? "",
                        TotalTicketsSold = ev.Tickets.Count(t => t.Status == TicketStatus.Sold),
                        AvailableSeats = ev.TotalSeat - ev.Tickets.Count(t => t.Status == TicketStatus.Sold)
                    })
                    .FirstOrDefaultAsync();
                if (eventDetails == null)
                {
                    throw new KeyNotFoundException("Event not found.");
                }
                return eventDetails;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving event details.", ex);
            }
        }
    }
}
