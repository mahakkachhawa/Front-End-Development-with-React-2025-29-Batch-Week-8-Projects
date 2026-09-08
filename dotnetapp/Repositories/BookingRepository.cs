using dotnetapp.Data;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Customer)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<List<Booking>> GetByCustomerAsync(
            string customerId)
        {
            return await _context.Bookings
                .Include(b => b.Event)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByEventAsync(
            int eventId)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .Where(b => b.EventId == eventId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByStatusAsync(
            string status)
        {
            return await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Customer)
                .Where(b => b.Status == status)
                .ToListAsync();
        }
    }
}