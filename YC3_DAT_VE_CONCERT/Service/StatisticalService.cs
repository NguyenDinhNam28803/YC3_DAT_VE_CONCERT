using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class StatisticalService : IStatisticalService
    {
        private readonly ApplicationDbContext _context;
        public StatisticalService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy dữ liệu thống kê
        public StatisticalDto GetStatisticalData()
        {
            throw new NotImplementedException();
        }
    }
}
