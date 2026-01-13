using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOsController : ControllerBase
    {
        private readonly IPayOSService _payOSService;
        public PayOsController(IPayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        [HttpPost("create-payment-link")]
        [SwaggerOperation(Summary = "Create a payment link", Description = "Creates a payment link using PayOS service")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePaymentLink([FromBody] PaymentLinkRequestModel request)
        {
            try
            {
                var amount_int = Convert.ToInt32(request.Amount); // Convert to smallest currency unit
                var result = await _payOSService.CreatePaymentLink(request.OrderCode, amount_int, request.Description, request.BuyerName, request.BuyerEmail);
                return Ok(new
                {
                    success = true,
                    Message = "Payment link created successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating payment link", Details = ex.Message });
            }
        }

        [HttpPost("Webhook")]
        public IActionResult PayOsWebhook([FromBody] PaymentWebhookData webhookData)
        {
            // Xử lý dữ liệu webhook ở đây
            // Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu dựa trên thông tin từ webhookData
            return Ok(); // Trả về mã trạng thái 200 để xác nhận đã nhận webhook
        }
    }
}
