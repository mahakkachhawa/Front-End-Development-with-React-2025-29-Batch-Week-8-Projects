using dotnetapp.DTOs;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Repositories;

namespace dotnetapp.Services
{
    public class WishlistService
    {
        private readonly IWishlistRepository _repository;

        public WishlistService(
            IWishlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EventDTO>> GetWishlistAsync(
            string customerId)
        {
            var wishlist =
                await _repository.GetByCustomerAsync(
                    customerId);

            return wishlist
                .Where(w => w.Event != null)
                .Select(w => new EventDTO
                {
                    Id = w.Event!.Id,
                    Name = w.Event.Name,
                    Description = w.Event.Description,
                    EventDate = w.Event.EventDate,
                    EventTime = w.Event.EventTime,
                    Venue = w.Event.Venue,
                    Capacity = w.Event.Capacity,
                    BookedSeats = w.Event.BookedSeats,
                    RemainingSeats =
                        w.Event.RemainingSeats,
                    TicketPrice = w.Event.TicketPrice,
                    EventImage = w.Event.EventImage,
                    CategoryId = w.Event.CategoryId,
                    CategoryName =
                        w.Event.Category?.Name,
                    Status = w.Event.Status
                })
                .ToList();
        }

        public async Task<Wishlist> AddToWishlistAsync(
            string customerId,
            int eventId)
        {
            var existing =
                await _repository.GetAsync(
                    customerId,
                    eventId);

            if (existing != null)
                throw new BusinessException(
                    "Event already exists in wishlist.");

            var wishlist = new Wishlist
            {
                CustomerId = customerId,
                EventId = eventId,
                AddedDate = DateTime.UtcNow
            };

            return await _repository.AddAsync(wishlist);
        }

        public async Task<bool> RemoveFromWishlistAsync(
            string customerId,
            int eventId)
        {
            return await _repository.RemoveAsync(
                customerId,
                eventId);
        }

        public async Task<bool> IsInWishlistAsync(
            string customerId,
            int eventId)
        {
            return await _repository.GetAsync(
                customerId,
                eventId) != null;
        }
    }
}