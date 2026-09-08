using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnetapp.Constants;

namespace dotnetapp.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string OrganizerId { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public TimeSpan EventTime { get; set; }

        [Required]
        [StringLength(250)]
        public string Venue { get; set; } = string.Empty;

        [Range(1, 1000000)]
        public int Capacity { get; set; }

        [Range(0, 1000000)]
        public int BookedSeats { get; set; }

        [Range(0, 10000000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TicketPrice { get; set; }

        public string? EventImage { get; set; }

        [Required]
        public string Status { get; set; } = EventStatuses.Active;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OrganizerId))]
        public ApplicationUser? Organizer { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public EventCategory? Category { get; set; }

        public ICollection<Booking> Bookings { get; set; } =
            new List<Booking>();

        public ICollection<Review> Reviews { get; set; } =
            new List<Review>();

        public ICollection<Wishlist> Wishlists { get; set; } =
            new List<Wishlist>();

        [NotMapped]
        public int RemainingSeats => Math.Max(0, Capacity - BookedSeats);
    }
}