using System;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IAuthService
    {
        Task Register(RegisterDto registerDto);
        Task<CustomerResponseDto> Login(LoginDto loginDto);
    }
}
