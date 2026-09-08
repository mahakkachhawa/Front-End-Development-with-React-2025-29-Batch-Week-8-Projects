using dotnetapp.Data;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Event)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Review> AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<List<Review>> GetByEventAsync(
            int eventId)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Review>> GetByCustomerAsync(
            string customerId)
        {
            return await _context.Reviews
                .Include(r => r.Event)
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(
            int eventId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.EventId == eventId)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings.Count == 0
                ? 0
                : ratings.Average();
        }
    }
}