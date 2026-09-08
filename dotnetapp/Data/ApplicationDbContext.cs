using dotnetapp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events => Set<Event>();

        public DbSet<EventCategory> EventCategories =>
            Set<EventCategory>();

        public DbSet<Booking> Bookings =>
            Set<Booking>();

        public DbSet<Review> Reviews =>
            Set<Review>();

        public DbSet<Wishlist> Wishlists =>
            Set<Wishlist>();

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasOne(w => w.Customer)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasOne(w => w.Event)
                .WithMany(e => e.Wishlists)
                .HasForeignKey(w => w.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasIndex(w => new
                {
                    w.CustomerId,
                    w.EventId
                })
                .IsUnique();

            builder.Entity<Review>()
                .HasIndex(r => new
                {
                    r.CustomerId,
                    r.EventId
                })
                .IsUnique();

            builder.Entity<Event>()
                .Property(e => e.TicketPrice)
                .HasPrecision(10, 2);

            builder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(10, 2);

            SeedCategories(builder);
        }

        private static void SeedCategories(
            ModelBuilder builder)
        {
            builder.Entity<EventCategory>().HasData(
                new EventCategory
                {
                    Id = 1,
                    Name = "Technology",
                    Description = "Technology related events"
                },
                new EventCategory
                {
                    Id = 2,
                    Name = "Music",
                    Description = "Music and concerts"
                },
                new EventCategory
                {
                    Id = 3,
                    Name = "Sports",
                    Description = "Sports events"
                },
                new EventCategory
                {
                    Id = 4,
                    Name = "Business",
                    Description = "Business and networking"
                },
                new EventCategory
                {
                    Id = 5,
                    Name = "Workshop",
                    Description = "Workshops and training"
                }
            );
        }
    }
}