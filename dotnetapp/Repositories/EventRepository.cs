using dotnetapp.Data;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Reviews)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Reviews)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> AddAsync(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<Event> UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Events.FindAsync(id);

            if (entity == null)
                return false;

            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Event>> SearchAsync(
            string? search,
            int? categoryId,
            string? location,
            DateTime? date,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy)
        {
            var query = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Name.Contains(search) ||
                    e.Description.Contains(search) ||
                    e.Venue.Contains(search));
            }

            if (categoryId.HasValue)
                query = query.Where(e =>
                    e.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(e =>
                    e.Venue.Contains(location));
            }

            if (date.HasValue)
            {
                query = query.Where(e =>
                    e.EventDate.Date == date.Value.Date);
            }

            if (minPrice.HasValue)
                query = query.Where(e =>
                    e.TicketPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(e =>
                    e.TicketPrice <= maxPrice.Value);

            query = sortBy?.ToLower() switch
            {
                "priceasc" =>
                    query.OrderBy(e => e.TicketPrice),

                "pricedesc" =>
                    query.OrderByDescending(e => e.TicketPrice),

                "name" =>
                    query.OrderBy(e => e.Name),

                "date" =>
                    query.OrderBy(e => e.EventDate),

                _ =>
                    query.OrderBy(e => e.EventDate)
            };

            return await query.ToListAsync();
        }

        public async Task<List<Event>> GetByOrganizerAsync(
            string organizerId)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Reviews)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.CreatedDate)
                .ToListAsync();
        }
    }
}