using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IVenueService
    {

        // Lấy danh sách tất cả địa điểm tổ chức
        List<VenueResponseDto> GetAllVenues();

        // Lấy địa điểm tổ chức theo ID
        VenueResponseDto GetVenueById(int venueId);

        // Lấy địa điểm tổ chức theo tên
        VenueResponseDto GetVenueByName(string name);

        // Tạo địa điểm tổ chức mới
        void CreateVenue(CreateVenueDto createVenueDto);

        // Cập nhật địa điểm tổ chức
        void UpdateVenue(int venueId, UpdateVenueDto updateVenueDto);

        // Xóa địa điểm tổ chức
        bool DeleteVenue(int venueId);
    }
}
