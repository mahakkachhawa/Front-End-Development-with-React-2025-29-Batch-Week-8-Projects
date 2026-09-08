using dotnetapp.Models;

namespace dotnetapp.Repositories
{
    public interface IWishlistRepository
    {
        Task<List<Wishlist>> GetByCustomerAsync(
            string customerId);

        Task<Wishlist?> GetAsync(
            string customerId,
            int eventId);

        Task<Wishlist> AddAsync(Wishlist wishlist);

        Task<bool> RemoveAsync(
            string customerId,
            int eventId);
    }
}