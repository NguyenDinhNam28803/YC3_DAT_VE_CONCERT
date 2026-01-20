using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PayOS;
using PayOS.Models.Webhooks;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;
using System.Text.Json;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOsController : ControllerBase
    {
        private readonly IPayOSService _payOSService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly PayOSClient _payOSClient;
        private readonly string _checksumKey;

        public PayOsController(IPayOSService payOSService, ApplicationDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _payOSService = payOSService;
            _context = context;
            _env = env;

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var clientId = configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("PayOS:ClientId");
            var apiKey = configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("PayOS:ApiKey");
            _checksumKey = configuration["PayOS:ChecksumKey"] ?? throw new ArgumentNullException("PayOS:ChecksumKey");

            var options = new PayOSOptions
            {
                ClientId = clientId,
                ApiKey = apiKey,
                ChecksumKey = _checksumKey
            };

            _payOSClient = new PayOSClient(options);
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

        [HttpPost("webhook")]
        [SwaggerOperation(Summary = "Handle PayOS webhook", Description = "Handles incoming webhooks from PayOS and verifies signature")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Webhook([FromBody] Webhook webhook)
        {
            try
            {
                // 1️⃣ Verify webhook bằng SDK
                var verified = _payOSClient.Webhooks.VerifyAsync(webhook);

                // 2️⃣ Kiểm tra thanh toán thành công
                if (!webhook.Success)
                    return BadRequest(new { 
                        success = false,
                        message= "Payment not completed" 
                    });

                // 3️⃣ Cập nhật đơn hàng
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == webhook.Data.OrderCode);

                if (order == null)
                    return NotFound("Order not found");

                order.Status = OrderStatus.Completed;
                order.TotalAmount = webhook.Data.Amount;
                order.PaymentLink = webhook.Data.Reference;

                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
