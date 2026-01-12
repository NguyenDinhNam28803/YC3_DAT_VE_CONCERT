using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Interface;
using Swashbuckle.AspNetCore.Annotations;
using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;
        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all venues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetAllVenues()
        {
            try
            {
                var venues = await _venueService.GetAllVenues();
                return Ok(new
                {
                    success = true,
                    message = "Venues retrieved successfully",
                    data = venues
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
        [Route("{venueId}")]
        [SwaggerOperation(Summary = "Get venue by ID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetVenueById(int venueId)
        {
            try
            {
                var venue = await _venueService.GetVenueById(venueId);
                return Ok(new
                {
                    success = true,
                    message = "Venue retrieved successfully",
                    data = venue
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
        [Route("name/{venueName}")]
        [SwaggerOperation(Summary = "Get venues by name")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetVenuesByName(string venueName)
        {
            try
            {
                var venues = await _venueService.GetVenueByName(venueName);
                return Ok(new
                {
                    success = true,
                    message = "Venues retrieved successfully",
                    data = venues
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
        [SwaggerOperation(Summary = "Create a new venue")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> CreateVenue([FromBody] CreateVenueDto venueDto)
        {
            try
            {
                var createdVenue = await _venueService.CreateVenue(venueDto);
                return CreatedAtAction(nameof(GetVenueById), new { venueId = createdVenue.Id }, new
                {
                    success = true,
                    message = "Venue created successfully",
                    data = createdVenue
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

        [HttpPut]
        [Route("{venueId}")]
        [SwaggerOperation(Summary = "Update an existing venue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> UpdateVenue(int venueId, [FromBody] UpdateVenueDto venueDto)
        {
            try
            {
                var updatedVenue = await _venueService.UpdateVenue(venueId, venueDto);
                return Ok(new
                {
                    success = true,
                    message = "Venue updated successfully",
                    data = updatedVenue
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

        [HttpDelete]
        [Route("{venueId}")]
        [SwaggerOperation(Summary = "Delete a venue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> DeleteVenue(int venueId)
        {
            try
            {
                await _venueService.DeleteVenue(venueId);
                return Ok(new
                {
                    success = true,
                    message = "Venue deleted successfully"
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
