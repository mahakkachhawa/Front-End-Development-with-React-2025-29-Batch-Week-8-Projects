using dotnetapp.Data;
using dotnetapp.DTOs;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _repository;
        private readonly ApplicationDbContext _context;

        public ReviewService(
            IReviewRepository repository,
            ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<ReviewDTO>> GetEventReviewsAsync(
            int eventId)
        {
            var reviews =
                await _repository.GetByEventAsync(eventId);

            return reviews.Select(MapToDTO).ToList();
        }

        public async Task<ReviewDTO> CreateReviewAsync(
            ReviewDTO dto,
            string customerId)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ValidationException(
                    "Rating must be between 1 and 5.");

            if (string.IsNullOrWhiteSpace(dto.Comment))
                throw new ValidationException(
                    "Review comment is required.");

            var customer =
                await _context.Users.FindAsync(customerId);

            if (customer == null)
                throw new NotFoundException(
                    "Customer not found.");

            var eventEntity =
                await _context.Events.FindAsync(dto.EventId);

            if (eventEntity == null)
                throw new NotFoundException(
                    "Event not found.");

            var existing =
                await _context.Reviews.FirstOrDefaultAsync(r =>
                    r.CustomerId == customerId &&
                    r.EventId == dto.EventId);

            if (existing != null)
                throw new BusinessException(
                    "You have already reviewed this event.");

            var review = new Review
            {
                CustomerId = customerId,
                EventId = dto.EventId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedDate = DateTime.UtcNow
            };

            var result =
                await _repository.AddAsync(review);

            result.Customer = customer;
            result.Event = eventEntity;

            return MapToDTO(result);
        }

        private static ReviewDTO MapToDTO(
            Review review)
        {
            return new ReviewDTO
            {
                Id = review.Id,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer?.Name,
                EventId = review.EventId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedDate = review.CreatedDate
            };
        }
    }
}