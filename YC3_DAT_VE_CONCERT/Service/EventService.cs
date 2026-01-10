using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using Microsoft.EntityFrameworkCore;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventResponseDto>> GetAllEvents()
        {
            try
            {
                List<EventResponseDto> events = await _context.Events
                    .Select(e => new EventResponseDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Date = e.Date,
                        TotalSeat = e.TotalSeat,
                        VenueName = e.Venue.Name,
                        VenueLocation = e.Venue.Location,
                        VenueCapacity = e.Venue.Capacity,
                        Description = e.Description,
                        TotalTicketsSold = e.Tickets.Count(t => t.Status == Model.TicketStatus.Sold),
                        AvailableSeats = e.TotalSeat - e.Tickets.Count(t => t.Status == Model.TicketStatus.Sold)
                    })
                    .ToListAsync();
                if (events == null || events.Count == 0)
                {
                    throw new Exception("No events found.");
                }
                return events;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving events.", ex);
            }
        }

        public async Task<EventResponseDto> GetEventById(int eventId)
        {
            throw new NotImplementedException();
        }

        public void CreateEvent(Model.Event newEvent)
        {
            throw new NotImplementedException();
        }

        public void UpdateEvent(int eventId, Model.Event updatedEvent)
        {
            throw new NotImplementedException();
        }

        public bool DeleteEvent(int eventId)
        {
            throw new NotImplementedException();
        }
    }
}
