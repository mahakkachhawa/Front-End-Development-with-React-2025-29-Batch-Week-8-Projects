using dotnetapp.Constants;
using dotnetapp.Data;
using dotnetapp.DTOs;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _repository;
        private readonly ApplicationDbContext _context;

        public BookingService(
            IBookingRepository repository,
            ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _repository.GetAllAsync();

            return bookings.Select(MapToDTO).ToList();
        }

        public async Task<BookingDTO?> GetBookingByIdAsync(int id)
        {
            var booking = await _repository.GetByIdAsync(id);

            return booking == null
                ? null
                : MapToDTO(booking);
        }

        public async Task<List<BookingDTO>> GetCustomerBookingsAsync(
            string customerId)
        {
            var bookings =
                await _repository.GetByCustomerAsync(customerId);

            return bookings.Select(MapToDTO).ToList();
        }

        public async Task<List<BookingDTO>> GetEventBookingsAsync(
            int eventId)
        {
            var bookings =
                await _repository.GetByEventAsync(eventId);

            return bookings.Select(MapToDTO).ToList();
        }

        public async Task<BookingDTO> CreateBookingAsync(
            BookingDTO dto,
            string customerId)
        {
            if (dto.TicketQuantity <= 0)
                throw new ValidationException(
                    "Ticket quantity must be greater than zero.");

            var customer =
                await _context.Users.FindAsync(customerId);

            if (customer == null)
                throw new NotFoundException(
                    "Customer not found.");

            if (!customer.IsActive)
                throw new BusinessException(
                    "Customer account is inactive.");

            var eventEntity =
                await _context.Events
                    .FirstOrDefaultAsync(e => e.Id == dto.EventId);

            if (eventEntity == null)
                throw new NotFoundException(
                    "Event not found.");

            if (eventEntity.Status != EventStatuses.Active)
                throw new BusinessException(
                    "This event is not available for booking.");

            if (eventEntity.EventDate <= DateTime.Now)
                throw new BusinessException(
                    "Cannot book a past event.");

            var remainingSeats =
                eventEntity.Capacity -
                eventEntity.BookedSeats;

            if (dto.TicketQuantity > remainingSeats)
                throw new BusinessException(
                    "Requested tickets exceed available seats.");

            // IMPORTANT:
            // Total is always calculated by backend.
            var totalAmount =
                dto.TicketQuantity *
                eventEntity.TicketPrice;

            eventEntity.BookedSeats += dto.TicketQuantity;

            var booking = new Booking
            {
                CustomerId = customerId,
                EventId = eventEntity.Id,
                TicketQuantity = dto.TicketQuantity,
                TotalAmount = totalAmount,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatuses.Confirmed,
                PaymentStatus = PaymentStatuses.Paid,
                BookingReference =
                    GenerateBookingReference()
            };

            _context.Events.Update(eventEntity);

            var result =
                await _repository.AddAsync(booking);

            return MapToDTO(result);
        }

        public async Task<BookingDTO> CancelBookingAsync(
            int id,
            string customerId)
        {
            var booking =
                await _repository.GetByIdAsync(id);

            if (booking == null)
                throw new NotFoundException(
                    "Booking not found.");

            if (booking.CustomerId != customerId)
                throw new BusinessException(
                    "You can only cancel your own booking.");

            if (booking.Status == BookingStatuses.Cancelled)
                throw new BusinessException(
                    "Booking is already cancelled.");

            if (booking.Event == null)
                throw new NotFoundException(
                    "Associated event not found.");

            booking.Event.BookedSeats =
                Math.Max(
                    0,
                    booking.Event.BookedSeats -
                    booking.TicketQuantity);

            booking.Status =
                BookingStatuses.Cancelled;

            booking.PaymentStatus =
                PaymentStatuses.Refunded;

            _context.Events.Update(booking.Event);

            await _repository.UpdateAsync(booking);

            return MapToDTO(booking);
        }

        public async Task<BookingDTO> UpdateBookingStatusAsync(
            int id,
            string status)
        {
            var booking =
                await _repository.GetByIdAsync(id);

            if (booking == null)
                throw new NotFoundException(
                    "Booking not found.");

            var allowed = new[]
            {
                BookingStatuses.Pending,
                BookingStatuses.Confirmed,
                BookingStatuses.Cancelled,
                BookingStatuses.Completed
            };

            if (!allowed.Contains(status))
                throw new ValidationException(
                    "Invalid booking status.");

            booking.Status = status;

            await _repository.UpdateAsync(booking);

            return MapToDTO(booking);
        }

        private static string GenerateBookingReference()
        {
            return "EVT-" +
                   DateTime.UtcNow
                       .ToString("yyyyMMddHHmmss") +
                   "-" +
                   Guid.NewGuid()
                       .ToString("N")
                       .Substring(0, 6)
                       .ToUpper();
        }

        private static BookingDTO MapToDTO(
            Booking booking)
        {
            return new BookingDTO
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                EventId = booking.EventId,
                EventName = booking.Event?.Name,
                EventDate = booking.Event?.EventDate,
                EventTime = booking.Event?.EventTime,
                Venue = booking.Event?.Venue,
                TicketPrice =
                    booking.Event?.TicketPrice ?? 0,
                TicketQuantity = booking.TicketQuantity,
                TotalAmount = booking.TotalAmount,
                BookingDate = booking.BookingDate,
                Status = booking.Status,
                PaymentStatus = booking.PaymentStatus,
                BookingReference =
                    booking.BookingReference
            };
        }
    }
}