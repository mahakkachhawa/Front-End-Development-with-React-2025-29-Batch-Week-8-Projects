using dotnetapp.Data;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Wishlist>> GetByCustomerAsync(
            string customerId)
        {
            return await _context.Wishlists
                .Include(w => w.Event)
                .ThenInclude(e => e!.Category)
                .Where(w => w.CustomerId == customerId)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetAsync(
            string customerId,
            int eventId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w =>
                    w.CustomerId == customerId &&
                    w.EventId == eventId);
        }

        public async Task<Wishlist> AddAsync(
            Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task<bool> RemoveAsync(
            string customerId,
            int eventId)
        {
            var wishlist = await GetAsync(
                customerId,
                eventId);

            if (wishlist == null)
                return false;

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}