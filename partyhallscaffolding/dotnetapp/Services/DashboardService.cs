using dotnetapp.Constants;
using dotnetapp.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetCustomerDashboardAsync(
            string customerId)
        {
            var bookings =
                await _context.Bookings
                    .Include(b => b.Event)
                    .Where(b => b.CustomerId == customerId)
                    .ToListAsync();

            return new
            {
                TotalBookings = bookings.Count,

                UpcomingBookings =
                    bookings.Count(b =>
                        b.Event != null &&
                        b.Event.EventDate >= DateTime.Now &&
                        b.Status != BookingStatuses.Cancelled),

                CompletedBookings =
                    bookings.Count(b =>
                        b.Status == BookingStatuses.Completed),

                CancelledBookings =
                    bookings.Count(b =>
                        b.Status == BookingStatuses.Cancelled),

                TotalSpent =
                    bookings
                        .Where(b =>
                            b.Status != BookingStatuses.Cancelled)
                        .Sum(b => b.TotalAmount)
            };
        }

        public async Task<object> GetOrganizerDashboardAsync(
            string organizerId)
        {
            var events =
                await _context.Events
                    .Where(e =>
                        e.OrganizerId == organizerId)
                    .ToListAsync();

            var eventIds =
                events.Select(e => e.Id).ToList();

            var bookings =
                await _context.Bookings
                    .Where(b =>
                        eventIds.Contains(b.EventId))
                    .ToListAsync();

            return new
            {
                TotalEvents = events.Count,

                ActiveEvents =
                    events.Count(e =>
                        e.Status == EventStatuses.Active),

                CompletedEvents =
                    events.Count(e =>
                        e.Status == EventStatuses.Completed),

                CancelledEvents =
                    events.Count(e =>
                        e.Status == EventStatuses.Cancelled),

                TotalRegistrations =
                    bookings.Count(b =>
                        b.Status != BookingStatuses.Cancelled),

                TotalRevenue =
                    bookings
                        .Where(b =>
                            b.Status != BookingStatuses.Cancelled)
                        .Sum(b => b.TotalAmount),

                TotalCapacity =
                    events.Sum(e => e.Capacity),

                BookedSeats =
                    events.Sum(e => e.BookedSeats)
            };
        }
    }
}