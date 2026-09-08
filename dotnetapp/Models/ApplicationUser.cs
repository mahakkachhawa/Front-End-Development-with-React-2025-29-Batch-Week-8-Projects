using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public ICollection<Event> OrganizedEvents { get; set; } =
            new List<Event>();

        public ICollection<Booking> Bookings { get; set; } =
            new List<Booking>();

        public ICollection<Review> Reviews { get; set; } =
            new List<Review>();

        public ICollection<Wishlist> Wishlists { get; set; } =
            new List<Wishlist>();
    }
}