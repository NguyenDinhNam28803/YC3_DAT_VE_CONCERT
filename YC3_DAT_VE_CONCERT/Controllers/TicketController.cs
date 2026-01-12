using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        // Các action method sẽ được định nghĩa ở đây, ví dụ:
        [HttpGet]
        [SwaggerOperation(Summary = "Get all tickets")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var tickets = await _ticketService.GetAllTicket();
                return Ok(new {
                    success = true,
                    message = "Tickets retrieved successfully",
                    data = tickets
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("user/{userId}")]
        [SwaggerOperation(Summary = "Get tickets by user ID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetTicketsByUserId(int userId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByUserId(userId);
                return Ok(new
                {
                    success = true,
                    message = "Tickets retrieved successfully",
                    data = tickets
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new ticket")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public IActionResult CreateTicket([FromBody] TicketDtoRequest ticket)
        {
            try
            {
                var ticketDetail = new TicketDtoRequest
                {
                    EventId = ticket.EventId,
                    SeatNumber = ticket.SeatNumber,
                    Price = ticket.Price,
                };
                var createdTicket = _ticketService.CreateTicket(ticketDetail);
                return Ok(new
                {
                    success = true,
                    message = "Ticket created successfully",
                    data = createdTicket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
