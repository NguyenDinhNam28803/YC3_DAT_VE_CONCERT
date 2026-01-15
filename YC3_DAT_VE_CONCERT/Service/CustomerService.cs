using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using BCrypt.Net;

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
        public async Task<List<Dto.CustomerResponseDto>> GetAllCustomers()
        {
            try
            {
                var listUser = await _context.Customers
                    .Include(u => u.Tickets)
                        .ThenInclude(t => t.Event)
                    .Include(u => u.Orders)
                        .ThenInclude(o => o.Tickets)
                    .Include(u => u.Role)
                    .Select(user => new Dto.CustomerResponseDto
                    {
                        Id = user.Id,
                        Role = user.Role.Name,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        TotalOrders = user.Orders.Count,
                        TotalTickets = user.Tickets.Count
                    })
                    .ToListAsync();

                return listUser;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving all customers.", ex);
            }
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
        public async Task<CustomerResponseDto> UpdateCustomer(int customerId, UpdateCustomerDto updateCustomerDto)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(customerId);
                if (existingCustomer == null)
                {
                    throw new Exception("Customer not found");
                }

                // Validate current password
                if (existingCustomer.Password != updateCustomerDto.CurrentPassword)
                {
                    throw new Exception("Current password is incorrect");
                }

                // Update customer details
                existingCustomer.Name = updateCustomerDto.Name;
                existingCustomer.Phone = updateCustomerDto.Phone;

                // Check current password before updating to new password
                if (!string.IsNullOrEmpty(updateCustomerDto.NewPassword))
                {
                    existingCustomer.Password = updateCustomerDto.NewPassword;
                }

                if (BCrypt.Net.BCrypt.Verify(updateCustomerDto.CurrentPassword, existingCustomer.Password))
                {
                    // Hash the new password before saving
                    existingCustomer.Password = BCrypt.Net.BCrypt.HashPassword(updateCustomerDto.NewPassword);
                }
                else
                {
                    throw new Exception("Current password is incorrect");
                }

                if (!string.IsNullOrEmpty(updateCustomerDto.NewPassword))
                {
                    if (updateCustomerDto.NewPassword != updateCustomerDto.ConfirmPassword)
                    {
                        throw new Exception("New password and confirm password do not match");
                    }
                    // Hash the new password before saving
                    existingCustomer.Password = BCrypt.Net.BCrypt.HashPassword(updateCustomerDto.NewPassword);
                }

                _context.Customers.Update(existingCustomer);
                await _context.SaveChangesAsync();

                var userInfo = new CustomerResponseDto
                {
                    Id = existingCustomer.Id,
                    Name = existingCustomer.Name,
                    Email = existingCustomer.Email,
                    Phone = existingCustomer.Phone,
                    Role = (await _context.Roles.FindAsync(existingCustomer.RoleId))?.Name ?? "Unknown",
                    TotalOrders = await _context.Orders.CountAsync(o => o.CustomerId == existingCustomer.Id),
                    TotalTickets = await _context.Tickets.CountAsync(t => t.CustomerId == existingCustomer.Id)
                };

                return userInfo;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException($"An error occurred while updating customer with ID {customerId}.", ex);
            }
        }

        public async Task<CustomerResponseDto> ChangePassword(int customerId, UpdatePasswordDto changePasswordDto)
        {
            try
            {
                if (changePasswordDto.CurrentPassword == null || changePasswordDto.NewPassword == null || changePasswordDto.ConfirmPassword == null)
                {
                    throw new Exception("Invalid infomation");
                }

                var exsitingUser = await _context.Customers
                    .Include(u => u.Tickets)
                        .ThenInclude(t => t.Event)
                    .Include(u => u.Orders)
                        .ThenInclude(o => o.Tickets)
                    .Include(u => u.Role)
                    .Where(user => user.Id == customerId)
                    .FirstOrDefaultAsync();
                if (exsitingUser == null) {
                    throw new Exception($"User with id {customerId} not found");
                }
                var checkCurrentPass = BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, exsitingUser.Password);
                if (checkCurrentPass && (changePasswordDto.NewPassword == changePasswordDto.ConfirmPassword))
                {
                    exsitingUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                }

                _context.Customers.Update(exsitingUser);
                await _context.SaveChangesAsync();

                var user_response = new CustomerResponseDto
                {
                    Id = exsitingUser.Id,
                    Role = exsitingUser.Role.Name,
                    Name = exsitingUser.Name,
                    Email = exsitingUser.Email,
                    Phone = exsitingUser.Phone,
                    TotalOrders = exsitingUser.Orders.Count(o => o.CustomerId == exsitingUser.Id),
                    TotalTickets = exsitingUser.Tickets.Count(t => t.CustomerId == exsitingUser.Id)
                };

                return user_response;
            }
            catch (Exception ex)
            {
                throw new Exception("Ann error occcur when you change password", ex);
            }
        }

    }
}
