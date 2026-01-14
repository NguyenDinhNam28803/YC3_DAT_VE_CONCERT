using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IEventService
    {
        // Lấy danh sách tất cả sự kiện
        Task<List<EventResponseDto>> GetAllEvents();

        // Lấy sự kiện theo ID
        Task<EventResponseDto> GetEventById(int eventId);

        // Tạo sự kiện mới
        Task<EventResponseDto> CreateEvent(CreateEventDto newEvent);

        // Cập nhật sự kiện
        Task<EventResponseDto> UpdateEvent(int eventId, UpdateEventDto updatedEvent);

        // Xóa sự kiện
        Task<bool> DeleteEvent(int eventId);
    }
}
