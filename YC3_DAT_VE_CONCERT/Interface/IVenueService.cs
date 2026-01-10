using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IVenueService
    {

        // Lấy danh sách tất cả địa điểm tổ chức
        List<string> GetAllVenues();

        // Lấy địa điểm tổ chức theo ID
        string GetVenueById(int venueId);

        // Lấy địa điểm tổ chức theo tên
        string GetVenueByName(string name);

        // Tạo địa điểm tổ chức mới
        void CreateVenue(CreateVenueDto createVenueDto);

        // Cập nhật địa điểm tổ chức
        void UpdateVenue(int venueId, UpdateVenueDto updateVenueDto);

        // Xóa địa điểm tổ chức
        bool DeleteVenue(int venueId);
    }
}
