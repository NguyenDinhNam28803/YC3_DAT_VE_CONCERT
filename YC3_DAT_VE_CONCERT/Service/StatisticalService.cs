using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
        public async Task<StatisticalDto> GetStatisticalData()
        {
            try
            {
                var ticketSold = await _context.Tickets.Where(t => t.Status == Model.TicketStatus.Sold).SumAsync(t => t.Price);
                var totalTicketsSold = await _context.Tickets.CountAsync(t => t.Status == Model.TicketStatus.Sold);
                var totalCustomers = await _context.Customers.CountAsync();
                var statisticalData = new StatisticalDto
                {
                    TotalTicketsSold = totalTicketsSold,
                    TotalRevenue = ticketSold,
                    TotalCustomers = totalCustomers
                };
                return statisticalData;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving statistical data.", ex);
            }
        }
    }
}
