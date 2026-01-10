using System;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IAuthService
    {
        // Chức năng đăng ký
        Task Register(RegisterDto registerDto);

        // Chức năng đăng nhập
        Task<CustomerResponseDto> Login(LoginDto loginDto);
    }
}
