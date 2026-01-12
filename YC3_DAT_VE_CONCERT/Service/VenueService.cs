using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Data;
using Microsoft.EntityFrameworkCore;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class VenueService : IVenueService
    {
        private readonly ApplicationDbContext _context;
        public VenueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VenueResponseDto>> GetAllVenues()
        {
            try
            {
                var listVenues = await _context.Venues
                    .Include(v => v.Events)
                        .ThenInclude(e => e.Tickets)
                    .Select(v => new VenueResponseDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Location = v.Location,
                        Capacity = v.Capacity,
                        TotalEvents = v.Events.Count,
                        UpcomingEvents = v.Events.Where(e => e.Date > DateTime.Now)
                            .Select(e => new EventResponseStatistical
                            {
                                Id = e.Id,
                                Name = e.Name,
                                Date = e.Date,
                                TotalTicket = e.TotalSeat,
                                TotalSoldTicket = e.Tickets.Where(t => t.Status == TicketStatus.Sold).Count(),
                                VenueId = e.VenueId
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return listVenues;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving venues.", ex);
            }
        }

        public async Task<VenueResponseDto> GetVenueById(int venueId)
        {
            try
            {
                var venue = await _context.Venues
                    .Where(v => v.Id == venueId)
                    .Select(v => new VenueResponseDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Location = v.Location,
                        Capacity = v.Capacity,
                        TotalEvents = v.Events.Count,
                        UpcomingEvents = v.Events.Where(e => e.Date > DateTime.Now)
                            .Select(e => new EventResponseStatistical
                            {
                                Id = e.Id,
                                Name = e.Name,
                                Date = e.Date,
                                TotalTicket = e.TotalSeat,
                                TotalSoldTicket = e.Tickets.Where(t => t.Status == TicketStatus.Sold).Count(),
                                VenueId = e.VenueId
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if(venue == null)
                {
                    throw new Exception($"Venue with ID {venueId} not found.");
                }

                return venue;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the venue by ID.", ex);
            }
        }

        public async Task<List<VenueResponseDto>> GetVenueByName(string name)
        {
            try
            {
                var venue = await _context.Venues
                    .Where(v => v.Name.Contains(name))
                    .Select(v => new VenueResponseDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Location = v.Location,
                        Capacity = v.Capacity,
                        TotalEvents = v.Events.Count,
                        UpcomingEvents = v.Events.Where(e => e.Date > DateTime.Now)
                            .Select(e => new EventResponseStatistical
                            {
                                Id = e.Id,
                                Name = e.Name,
                                Date = e.Date,
                                TotalTicket = e.TotalSeat,
                                TotalSoldTicket = e.Tickets.Where(t => t.Status == TicketStatus.Sold).Count(),
                                VenueId = e.VenueId
                            })
                            .ToList()
                    })
                    .ToListAsync();

                if (venue == null)
                {
                    throw new Exception($"Venue with name {name} not found.");
                }

                return venue;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the venue by ID.", ex);
            }
        }

        public async Task<VenueResponseDto> CreateVenue(CreateVenueDto createVenueDto)
        {
            try
            {
                // validate data
                var existingVenue = await _context.Venues
                    .FirstOrDefaultAsync(v => v.Name == createVenueDto.Name);
                if (existingVenue != null)
                {
                    throw new Exception($"Venue with name {createVenueDto.Name} already exists.");
                }

                if (createVenueDto.Capacity <= 0)
                {
                    throw new Exception("Capacity must be greater than zero.");
                }

                var newVenue = new Venue
                {
                    Name = createVenueDto.Name,
                    Location = createVenueDto.Location,
                    Capacity = createVenueDto.Capacity
                };

                _context.Venues.Add(newVenue);
                _context.SaveChanges();

                var venueResponse = new VenueResponseDto
                {
                    Id = newVenue.Id,
                    Name = newVenue.Name,
                    Location = newVenue.Location,
                    Capacity = newVenue.Capacity,
                    TotalEvents = 0,
                    UpcomingEvents = new List<EventResponseStatistical>()
                };
                return venueResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the venue.", ex);
            }
        }

        public async Task<VenueResponseDto> UpdateVenue(int venueId, UpdateVenueDto updateVenueDto)
        {
            try
            {
                var venue = await _context.Venues.FirstOrDefaultAsync(v => v.Id == venueId);
                if (venue == null)
                {
                    throw new Exception($"Venue with ID {venueId} not found.");
                }

                venue.Name = updateVenueDto.Name;
                venue.Location = updateVenueDto.Location;
                venue.Capacity = updateVenueDto.Capacity;
                _context.Venues.Update(venue);
                await _context.SaveChangesAsync();
                var venueResponse = new VenueResponseDto
                {
                    Id = venue.Id,
                    Name = venue.Name,
                    Location = venue.Location,
                    Capacity = venue.Capacity,
                    TotalEvents = venue.Events.Count,
                    UpcomingEvents = venue.Events.Where(e => e.Date > DateTime.Now)
                            .Select(e => new EventResponseStatistical
                            {
                                Id = e.Id,
                                Name = e.Name,
                                Date = e.Date,
                                TotalTicket = e.TotalSeat,
                                TotalSoldTicket = e.Tickets.Where(t => t.Status == TicketStatus.Sold).Count(),
                                VenueId = e.VenueId
                            })
                            .ToList()
                };
                return venueResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the venue.", ex);
            }
        }

        public async Task<bool> DeleteVenue(int venueId)
        {
            try
            {
                var venue = _context.Venues.FirstOrDefault(v => v.Id == venueId);
                if (venue == null)
                {
                    throw new Exception($"Venue with ID {venueId} not found.");
                }

                var hasEvents = _context.Events.Any(e => e.VenueId == venueId);
                if (hasEvents)
                {
                    throw new Exception("Cannot delete venue with associated events.");
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the venue.", ex);
            }
        }
    }
}
