using dotnetapp.Constants;
using dotnetapp.DTOs;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Repositories;

namespace dotnetapp.Services
{
    public class EventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EventDTO>> GetAllEventsAsync()
        {
            var events = await _repository.GetAllAsync();

            return events.Select(MapToDTO).ToList();
        }

        public async Task<EventDTO?> GetEventByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            return entity == null
                ? null
                : MapToDTO(entity);
        }

        public async Task<EventDTO> AddEventAsync(
            EventDTO dto,
            string organizerId)
        {
            ValidateEvent(dto);

            var entity = new Event
            {
                OrganizerId = organizerId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                EventTime = dto.EventTime,
                Venue = dto.Venue,
                Capacity = dto.Capacity,
                BookedSeats = 0,
                TicketPrice = dto.TicketPrice,
                EventImage = dto.EventImage,
                Status = EventStatuses.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _repository.AddAsync(entity);

            return MapToDTO(result);
        }

        public async Task<EventDTO> UpdateEventAsync(
            int id,
            EventDTO dto,
            string organizerId)
        {
            ValidateEvent(dto);

            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                throw new NotFoundException("Event not found.");

            if (entity.OrganizerId != organizerId)
                throw new BusinessException(
                    "You can only modify your own events.");

            entity.CategoryId = dto.CategoryId;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.EventDate = dto.EventDate;
            entity.EventTime = dto.EventTime;
            entity.Venue = dto.Venue;
            entity.Capacity = dto.Capacity;
            entity.TicketPrice = dto.TicketPrice;
            entity.EventImage = dto.EventImage;

            var result = await _repository.UpdateAsync(entity);

            return MapToDTO(result);
        }

        public async Task<bool> DeleteEventAsync(
            int id,
            string organizerId)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                throw new NotFoundException("Event not found.");

            if (entity.OrganizerId != organizerId)
                throw new BusinessException(
                    "You can only delete your own events.");

            return await _repository.DeleteAsync(id);
        }

        public async Task<List<EventDTO>> SearchEventsAsync(
            string? search,
            int? categoryId,
            string? location,
            DateTime? date,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy)
        {
            var events = await _repository.SearchAsync(
                search,
                categoryId,
                location,
                date,
                minPrice,
                maxPrice,
                sortBy);

            return events.Select(MapToDTO).ToList();
        }

        public async Task<List<EventDTO>> GetOrganizerEventsAsync(
            string organizerId)
        {
            var events =
                await _repository.GetByOrganizerAsync(organizerId);

            return events.Select(MapToDTO).ToList();
        }

        private static void ValidateEvent(EventDTO dto)
        {
            if (dto.EventDate <= DateTime.Now)
                throw new ValidationException(
                    "Event date must be in the future.");

            if (dto.Capacity <= 0)
                throw new ValidationException(
                    "Event capacity must be greater than zero.");

            if (dto.TicketPrice < 0)
                throw new ValidationException(
                    "Ticket price cannot be negative.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException(
                    "Event name is required.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ValidationException(
                    "Event description is required.");

            if (string.IsNullOrWhiteSpace(dto.Venue))
                throw new ValidationException(
                    "Event venue is required.");
        }

        private static EventDTO MapToDTO(Event entity)
        {
            return new EventDTO
            {
                Id = entity.Id,
                OrganizerId = entity.OrganizerId,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.Name,
                Name = entity.Name,
                Description = entity.Description,
                EventDate = entity.EventDate,
                EventTime = entity.EventTime,
                Venue = entity.Venue,
                Capacity = entity.Capacity,
                BookedSeats = entity.BookedSeats,
                RemainingSeats = entity.RemainingSeats,
                TicketPrice = entity.TicketPrice,
                EventImage = entity.EventImage,
                Status = entity.Status,
                OrganizerName = entity.Organizer?.Name,
                AverageRating =
                    entity.Reviews?.Any() == true
                        ? entity.Reviews.Average(r => r.Rating)
                        : 0,
                TotalReviews =
                    entity.Reviews?.Count ?? 0,
                CreatedDate = entity.CreatedDate
            };
        }
    }
}