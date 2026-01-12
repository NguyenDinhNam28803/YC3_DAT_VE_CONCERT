using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IVenueService
    {

        // Lấy danh sách tất cả địa điểm tổ chức
        Task<List<VenueResponseDto>> GetAllVenues();

        // Lấy địa điểm tổ chức theo ID
        Task<VenueResponseDto> GetVenueById(int venueId);

        // Lấy địa điểm tổ chức theo tên
        Task<List<VenueResponseDto>> GetVenueByName(string name);

        // Tạo địa điểm tổ chức mới
        Task<VenueResponseDto> CreateVenue(CreateVenueDto createVenueDto);

        // Cập nhật địa điểm tổ chức
        Task<VenueResponseDto> UpdateVenue(int venueId, UpdateVenueDto updateVenueDto);

        // Xóa địa điểm tổ chức
        Task<bool> DeleteVenue(int venueId);
    }
}
