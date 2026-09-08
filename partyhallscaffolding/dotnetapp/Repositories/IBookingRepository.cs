using dotnetapp.Models;

namespace dotnetapp.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();

        Task<Booking?> GetByIdAsync(int id);

        Task<Booking> AddAsync(Booking booking);

        Task<Booking> UpdateAsync(Booking booking);

        Task<List<Booking>> GetByCustomerAsync(
            string customerId);

        Task<List<Booking>> GetByEventAsync(
            int eventId);

        Task<List<Booking>> GetByStatusAsync(
            string status);
    }
}