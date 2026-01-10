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
        void CreateEvent(Event newEvent);

        // Cập nhật sự kiện
        void UpdateEvent(int eventId, Event updatedEvent);

        // Xóa sự kiện
        bool DeleteEvent(int eventId);
    }
}
