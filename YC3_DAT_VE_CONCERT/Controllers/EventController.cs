using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Service;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all events")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllEvents();
                return Ok(new
                {
                    success = true,
                    message = "Events retrieved successfully",
                    data = events
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to retrieve events",
                    detail = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("{eventId}")]
        [SwaggerOperation(Summary = "Get event by ID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            try
            {
                var eventData = await _eventService.GetEventById(eventId);
                return Ok(new
                {
                    success = true,
                    message = "Event retrieved successfully",
                    data = eventData
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to retrieve event",
                    detail = ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new event")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto eventDto)
        {
            try
            {
                var createdEvent = await _eventService.CreateEvent(eventDto);
                return CreatedAtAction(nameof(GetEventById), new { eventId = createdEvent.Id }, new
                {
                    success = true,
                    message = "Event created successfully",
                    data = createdEvent
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to create event",
                    detail = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("{eventId}")]
        [SwaggerOperation(Summary = "Update an existing event")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] UpdateEventDto eventDto)
        {
            try
            {
                var existingEvent = await _eventService.GetEventById(eventId);
                if (existingEvent == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Event not found"
                    });
                }
                var updatedEvent = await _eventService.UpdateEvent(eventId, eventDto);
                return Ok(new
                {
                    success = true,
                    message = "Event updated successfully",
                    data = updatedEvent
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to update event",
                    detail = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("{eventId}")]
        [SwaggerOperation(Summary = "Delete an event")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            try
            {
                var existingEvent = await _eventService.GetEventById(eventId);
                if (existingEvent == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Event not found"
                    });
                }

                await _eventService.DeleteEvent(eventId);
                return Ok(new
                {
                    success = true,
                    message = "Event deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to delete event",
                    detail = ex.Message
                });
            }
        }
    }
}
