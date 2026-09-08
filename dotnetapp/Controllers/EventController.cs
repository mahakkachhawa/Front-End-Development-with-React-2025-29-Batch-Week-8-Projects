using System.Security.Claims;
using dotnetapp.Constants;
using dotnetapp.DTOs;
using dotnetapp.Exceptions;
using dotnetapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly EventService _service;

        public EventController(EventService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllEvents()
        {
            return Ok(
                await _service.GetAllEventsAsync());
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(
            int id)
        {
            var result =
                await _service.GetEventByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchEvents(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] string? location,
            [FromQuery] DateTime? date,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy)
        {
            return Ok(
                await _service.SearchEventsAsync(
                    search,
                    categoryId,
                    location,
                    date,
                    minPrice,
                    maxPrice,
                    sortBy));
        }

        [Authorize(Roles = UserRoles.Organizer)]
        [HttpGet("organizer/my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var organizerId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (organizerId == null)
                return Unauthorized();

            return Ok(
                await _service.GetOrganizerEventsAsync(
                    organizerId));
        }

        [Authorize(Roles = UserRoles.Organizer)]
        [HttpPost]
        public async Task<IActionResult> AddEvent(
            [FromBody] EventDTO dto)
        {
            try
            {
                var organizerId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (organizerId == null)
                    return Unauthorized();

                var result =
                    await _service.AddEventAsync(
                        dto,
                        organizerId);

                return CreatedAtAction(
                    nameof(GetEventById),
                    new { id = result.Id },
                    result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = UserRoles.Organizer)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] EventDTO dto)
        {
            try
            {
                var organizerId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (organizerId == null)
                    return Unauthorized();

                return Ok(
                    await _service.UpdateEventAsync(
                        id,
                        dto,
                        organizerId));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = UserRoles.Organizer)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(
            int id)
        {
            try
            {
                var organizerId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (organizerId == null)
                    return Unauthorized();

                await _service.DeleteEventAsync(
                    id,
                    organizerId);

                return Ok(new
                {
                    message = "Event deleted successfully."
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}