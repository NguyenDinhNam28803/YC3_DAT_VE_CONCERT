using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mail;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Send a test email", Description = "Sends a test email to the specified address")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> SendEmail([FromBody] EmailDtoRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request. 'email' is required in request body."
                });
            }

            // Validate email format
            try
            {
                _ = new MailAddress(request.Email);
            }
            catch
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid email format."
                });
            }
            try
            {
                await _emailService.SendEmail("Nam Nguyen",request.Email, "Test Email", "This is a test email from YC3_DAT_VE_CONCERT.");
                return Ok(new
                {
                    success = true,
                    message = "Email sent successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to send email",
                    detail = ex.Message
                });
            }
        }
    }
}
