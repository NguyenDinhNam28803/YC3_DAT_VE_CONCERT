using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        public AuthService(ApplicationDbContext context) 
        { 
            _context = context;
        }

        // Chức năng đăng nhập
        public async Task<CustomerResponseDto> Login(LoginDto loginDto)
        {
            try
            {
                var userName = await _context.Customers.FirstOrDefaultAsync(user => user.Name == loginDto.Username || user.Email == loginDto.Username);
                if (userName == null)
                {
                    throw new Exception("Invalid Username");
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, userName.Password);
                if (!isPasswordValid)
                {
                    throw new Exception("Invalid Password");
                }

                // Đăng nhập thành công, thực hiện các thao tác tiếp theo (nếu cần)
                var userInfo = await _context.Customers
                    .Include(u => u.Tickets)
                        .ThenInclude(t => t.Event)
                    .Include(u => u.Orders)
                        .ThenInclude(o => o.Tickets)
                    .Include(u => u.Role)
                    .Where(user => user.Name == loginDto.Username)
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

                if (userInfo == null)
                {
                    throw new Exception("User information not found");
                }
                return userInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message);
            }
        }

        // Chức năng đăng ký
        public async Task Register(RegisterDto registerDto)
        {
            try
            {
                var existingUser = _context.Customers.FirstOrDefault(user => user.Name == registerDto.Name);
                if (existingUser != null)
                {
                    throw new Exception("Username already exists");
                }

                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                    throw new Exception("Passwords do not match");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                var newUser = new Customer
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Phone = registerDto.Phone,
                    Password = hashedPassword,
                    RoleId = 2 // Mặc định là vai trò khách hàng
                };
                _context.Customers.Add(newUser);
                _context.SaveChanges();
                LoginDto loginDto = new LoginDto
                {
                    Username = registerDto.Name,
                    Password = registerDto.Password
                };
                await Login(loginDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during registration: " + ex.Message);
            }
        }
    }
}
