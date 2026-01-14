using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using Microsoft.EntityFrameworkCore;
using YC3_DAT_VE_CONCERT.Model;

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
            try
            {
                var eventexsiting = await _context.Events
                    .Where(ev => ev.Id == eventId)
                    .Select(ev => new EventResponseDto
                    {
                        Id = ev.Id,
                        Name = ev.Name,
                        Date = ev.Date,
                        TotalSeat = ev.TotalSeat,
                        VenueName = ev.Venue.Name,
                        VenueLocation = ev.Venue.Location,
                        VenueCapacity = ev.Venue.Capacity,
                        Description = ev.Description,
                        TotalTicketsSold = ev.Tickets.Count(t => t.Status == TicketStatus.Sold),
                        AvailableSeats = ev.TotalSeat - ev.Tickets.Count(t => t.Status == Model.TicketStatus.Sold)
                    })
                    .FirstOrDefaultAsync();

                if (eventexsiting == null)
                {
                    throw new Exception($"Event with ID {eventId} not found.");
                }

                return eventexsiting;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the event.", ex);
            }
        }

        public async Task<EventResponseDto> CreateEvent(CreateEventDto newEvent)
        {
            try
            {
                var existingVenue = await _context.Venues.FindAsync(newEvent.VenueId);
                if (existingVenue == null)
                {
                    throw new Exception($"Venue with ID {newEvent.VenueId} does not exist.");
                }

                if(newEvent.TotalSeat > existingVenue.Capacity)
                {
                    throw new Exception("Total seats for the event cannot exceed the venue's capacity.");
                }

                if (newEvent.Date < DateTime.Now)
                {
                    throw new Exception("Event date cannot be in the past.");
                }

                if (string.IsNullOrWhiteSpace(newEvent.Name))
                {
                    throw new Exception("Event name cannot be empty.");
                }

                if (newEvent.TotalSeat <= 0)
                {
                    throw new Exception("Total seats must be a positive number.");
                }

                var eventEntity = new Event
                {
                    Name = newEvent.Name,
                    Date = newEvent.Date,
                    VenueId = newEvent.VenueId,
                    Description = newEvent.Description,
                    TotalSeat = newEvent.TotalSeat
                };

                _context.Events.Add(eventEntity);
                await _context.SaveChangesAsync();

                return new EventResponseDto
                {
                    Id = eventEntity.Id,
                    Name = eventEntity.Name,
                    Date = eventEntity.Date,
                    TotalSeat = eventEntity.TotalSeat,
                    VenueName = existingVenue.Name,
                    VenueLocation = existingVenue.Location,
                    VenueCapacity = existingVenue.Capacity,
                    Description = eventEntity.Description,
                    TotalTicketsSold = 0,
                    AvailableSeats = eventEntity.TotalSeat
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the event.", ex);
            }
        }

        public async Task<EventResponseDto> UpdateEvent(int eventId, UpdateEventDto updatedEvent)
        {
            try
            {
                var eventEntity = await _context.Events.FindAsync(eventId);
                if (eventEntity == null)
                {
                    throw new Exception($"Event with ID {eventId} not found.");
                }
                var existingVenue = await _context.Venues.FindAsync(updatedEvent.VenueId);
                if (existingVenue == null)
                {
                    throw new Exception($"Venue with ID {updatedEvent.VenueId} does not exist.");
                }

                if (updatedEvent.TotalSeat > existingVenue.Capacity)
                {
                    throw new Exception("Total seats for the event cannot exceed the venue's capacity.");
                }

                if (updatedEvent.Date < DateTime.Now)
                {
                    throw new Exception("Event date cannot be in the past.");
                }

                if (string.IsNullOrWhiteSpace(updatedEvent.Name))
                {
                    throw new Exception("Event name cannot be empty.");
                }

                if (updatedEvent.TotalSeat <= 0)
                {
                    throw new Exception("Total seats must be a positive number.");
                }

                eventEntity.Name = updatedEvent.Name;
                eventEntity.Date = updatedEvent.Date;
                eventEntity.TotalSeat = updatedEvent.TotalSeat;
                eventEntity.VenueId = updatedEvent.VenueId;
                eventEntity.Description = updatedEvent.Description;
                await _context.SaveChangesAsync();
                return new EventResponseDto
                {
                    Id = eventEntity.Id,
                    Name = eventEntity.Name,
                    Date = eventEntity.Date,
                    TotalSeat = eventEntity.TotalSeat,
                    VenueName = existingVenue.Name,
                    VenueLocation = existingVenue.Location,
                    VenueCapacity = existingVenue.Capacity,
                    Description = eventEntity.Description,
                    TotalTicketsSold = eventEntity.Tickets.Count(t => t.Status == TicketStatus.Sold),
                    AvailableSeats = eventEntity.TotalSeat - eventEntity.Tickets.Count(t => t.Status == TicketStatus.Sold)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the event.", ex);
            }
        }

        public async Task<bool> DeleteEvent(int eventId)
        {
            try
            {
                var eventEntity = await _context.Events
                    .Include(e => e.Tickets)
                    .FirstOrDefaultAsync(e => e.Id == eventId);

                if (eventEntity == null)
                {
                    throw new Exception($"Event with ID {eventId} not found.");
                }
                if (eventEntity.Tickets.Any(t => t.Status == TicketStatus.Sold))
                {
                    throw new Exception("Cannot delete event with sold tickets.");
                }
                _context.Events.Remove(eventEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the event.", ex);
            }
        }
    }
}
