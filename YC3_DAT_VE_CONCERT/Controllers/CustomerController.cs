using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{customerId}")]
        [SwaggerOperation(Summary = "Get customer information by ID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public IActionResult GetUserInfo(int customerId)
        {
            try
            {
                var customer = _customerService.GetCustomerById(customerId);
                return Ok(new
                {
                    success = true,
                    message = "Customer retrieved successfully",
                    data = customer
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to retrieve customer",
                    detail = ex.Message
                });
            }
        }
    }
}
