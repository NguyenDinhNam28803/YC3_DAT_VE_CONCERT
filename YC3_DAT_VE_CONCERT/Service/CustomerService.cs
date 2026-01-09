using YC3_DAT_VE_CONCERT.Interface;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class CustomerService : ICustomerService
    {
        // Lấy danh sách tất cả khách hàng
        public List<Dto.CustomerResponseDto> GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        // Lấy thông tin khách hàng theo ID
        public Dto.CustomerResponseDto GetCustomerById(int customerId)
        {
            throw new NotImplementedException();
        }

        // Cập nhật thông tin khách hàng
        public void UpdateCustomer(int customerId, Dto.UpdateCustomerDto updateCustomerDto)
        {
            throw new NotImplementedException();
        }
    }
}
