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

        public Task<VenueResponseDto> GetVenueById(int venueId)
        {
            throw new NotImplementedException();
        }

        public Task<VenueResponseDto> GetVenueByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<VenueResponseDto> CreateVenue(CreateVenueDto createVenueDto)
        {
            throw new NotImplementedException();
        }

        public Task<VenueResponseDto> UpdateVenue(int venueId, UpdateVenueDto updateVenueDto)
        {
            throw new NotImplementedException();
        }

        public bool DeleteVenue(int venueId)
        {
            throw new NotImplementedException();
        }
    }
}
