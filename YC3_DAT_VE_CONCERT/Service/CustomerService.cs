using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class CustomerService : ICustomerService
    {
        // Lấy danh sách tất cả khách hàng
        private readonly ApplicationDbContext _context;
        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Dto.CustomerResponseDto> GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        // Lấy thông tin khách hàng theo ID
        public async Task<CustomerResponseDto> GetCustomerById(int customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(u => u.Tickets)
                        .ThenInclude(t => t.Event)
                    .Include(u => u.Orders)
                        .ThenInclude(o => o.Tickets)
                    .Include(u => u.Role)
                    .Where(user => user.Id == customerId)
                    .Select(user => new CustomerResponseDto
                    {
                        Id = user.Id,
                        Role = user.Role.Name,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        TotalOrders = user.Orders.Count,
                        TotalTickets = user.Tickets.Count
                    })
                    .FirstOrDefaultAsync();
                if (customer == null)
                {
                    throw new Exception("Customer not found");
                }
                return customer;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException($"An error occurred while retrieving customer with ID {customerId}.", ex);
            }
        }

        // Cập nhật thông tin khách hàng
        public void UpdateCustomer(int customerId, Dto.UpdateCustomerDto updateCustomerDto)
        {
            throw new NotImplementedException();
        }
    }
}
