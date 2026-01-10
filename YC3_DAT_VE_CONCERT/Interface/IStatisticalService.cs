using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IStatisticalService
    {
        // Lấy dữ liệu thống kê
        Task<StatisticalDto> GetStatisticalData();
    }
}
