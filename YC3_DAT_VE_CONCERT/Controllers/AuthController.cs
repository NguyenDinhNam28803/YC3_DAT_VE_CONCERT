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
        private readonly IEmailService _emailService;
        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
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
                await _emailService.SendEmail(
                    request.Name,
                    request.Email,
                    "🎉 Chào mừng bạn đến với YC3 DAT VE CONCERT",
                    @"<p>Tài khoản của bạn đã được tạo thành công!</p>
                    <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>YC3 DAT VE CONCERT</strong>. Giờ đây bạn có thể:</p>
                    <ul style='line-height: 1.8;'>
                        <li>🎫 Đặt vé các concert yêu thích</li>
                        <li>⭐ Lưu các sự kiện quan tâm</li>
                        <li>🔔 Nhận thông báo về concert mới</li>
                        <li>💳 Quản lý đơn hàng của bạn</li>
                    </ul>
                    <p style='margin-top: 20px;'>Hãy bắt đầu khám phá và đặt vé ngay hôm nay!</p>
                    <p style='color: #6b7280; font-size: 14px; margin-top: 24px;'>
                        <strong>Lưu ý:</strong> Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email hoặc liên hệ với chúng tôi ngay.
                    </p>"
                );
                return Ok(new
                {
                    success = true,
                    message = "Registration successful, Please login again!"
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var userInfo = await _authService.Login(request);
                await _emailService.SendEmail(
                    userInfo.Name,
                    userInfo.Email,
                    "🔐 Đăng nhập thành công - Chào mừng trở lại!",
                    $@"<p>Bạn vừa đăng nhập vào hệ thống <strong>YC3 DAT VE CONCERT</strong> thành công.</p>
                    <p><strong>Thông tin đăng nhập:</strong></p>
                    <table style='width: 100%; margin: 16px 0;'>
                        <tr>
                            <td style='padding: 8px 0; font-weight: 600; width: 120px;'>Thời gian:</td>
                            <td style='padding: 8px 0;'>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; font-weight: 600;'>Tài khoản:</td>
                            <td style='padding: 8px 0;'>{userInfo.Email}</td>
                        </tr>
                    </table>
                    <p style='color: #dc2626; background: #fef2f2; padding: 12px; border-radius: 6px; font-size: 14px; margin-top: 20px;'>
                        <strong>⚠️ Lưu ý bảo mật:</strong> Nếu bạn không thực hiện đăng nhập này, vui lòng đổi mật khẩu ngay lập tức hoặc liên hệ với chúng tôi.
                    </p>"
                ); 
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
