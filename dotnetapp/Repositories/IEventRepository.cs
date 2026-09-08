using dotnetapp.Models;

namespace dotnetapp.Repositories
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync();

        Task<Event?> GetByIdAsync(int id);

        Task<Event> AddAsync(Event eventEntity);

        Task<Event> UpdateAsync(Event eventEntity);

        Task<bool> DeleteAsync(int id);

        Task<List<Event>> SearchAsync(
            string? search,
            int? categoryId,
            string? location,
            DateTime? date,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy);

        Task<List<Event>> GetByOrganizerAsync(
            string organizerId);
    }
}