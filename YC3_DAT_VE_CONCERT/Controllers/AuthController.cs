using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Service;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Đăng ký", Description = "Đăng ký tài khoản cho user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                await _authService.Register(request);
                return Ok(new
                {
                    success = true,
                    message = "Registration successful"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Registration failed",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Đăng nhập", Description = "Đăng nhập và trả về thông tin người dùng")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var userInfo = await _authService.Login(request);
                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    data = userInfo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Login failed",
                    details = ex.Message
                });
            }
        }
    }
}
