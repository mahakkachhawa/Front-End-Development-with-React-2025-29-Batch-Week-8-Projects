using dotnetapp.Models;

namespace dotnetapp.Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();

        Task<Review?> GetByIdAsync(int id);

        Task<Review> AddAsync(Review review);

        Task<List<Review>> GetByEventAsync(int eventId);

        Task<List<Review>> GetByCustomerAsync(string customerId);

        Task<double> GetAverageRatingAsync(int eventId);
    }
}