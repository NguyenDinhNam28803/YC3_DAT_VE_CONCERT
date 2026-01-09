using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IVenueService
    {
        List<string> GetAllVenues();
        string GetVenueById(int venueId);
        string GetVenueByName(string name);
        void CreateVenue(CreateVenueDto createVenueDto);
        void UpdateVenue(int venueId, UpdateVenueDto updateVenueDto);
        bool DeleteVenue(int venueId);
    }
}
