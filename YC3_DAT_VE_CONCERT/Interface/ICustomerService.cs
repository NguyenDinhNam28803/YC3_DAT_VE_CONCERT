using System;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface ICustomerService
    {
        // Lấy danh sách tất cả khách hàng
        Task<List<Dto.CustomerResponseDto>> GetAllCustomers();

        // Lấy thông tin khách hàng theo ID
        Task<CustomerResponseDto> GetCustomerById(int customerId);

        // Cập nhật thông tin khách hàng
        Task<object> UpdateCustomer(int customerId, UpdateCustomerDto updateCustomerDto);
    }
}
