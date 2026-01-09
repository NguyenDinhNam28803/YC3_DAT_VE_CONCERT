using System;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface ICustomerService
    {
        List<CustomerResponseDto> GetAllCustomers();
        CustomerResponseDto GetCustomerById(int customerId);
        void UpdateCustomer(int customerId, UpdateCustomerDto updateCustomerDto);
    }
}
