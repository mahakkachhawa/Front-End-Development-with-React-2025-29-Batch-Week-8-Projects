using System.Security.Claims;
using dotnetapp.Constants;
using dotnetapp.DTOs;
using dotnetapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _service;

        public BookingController(
            BookingService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<IActionResult> CreateBooking(
            [FromBody] BookingDTO dto)
        {
            try
            {
                var customerId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (customerId == null)
                    return Unauthorized();

                var result =
                    await _service.CreateBookingAsync(
                        dto,
                        customerId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("my")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (customerId == null)
                return Unauthorized();

            return Ok(
                await _service.GetCustomerBookingsAsync(
                    customerId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBooking(
            int id)
        {
            var result =
                await _service.GetBookingByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id:int}/cancel")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<IActionResult> CancelBooking(
            int id)
        {
            try
            {
                var customerId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (customerId == null)
                    return Unauthorized();

                return Ok(
                    await _service.CancelBookingAsync(
                        id,
                        customerId));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("event/{eventId:int}")]
        [Authorize(Roles = UserRoles.Organizer)]
        public async Task<IActionResult> GetEventBookings(
            int eventId)
        {
            return Ok(
                await _service.GetEventBookingsAsync(
                    eventId));
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = UserRoles.Organizer)]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromQuery] string status)
        {
            try
            {
                return Ok(
                    await _service.UpdateBookingStatusAsync(
                        id,
                        status));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}