using System.ComponentModel.DataAnnotations;

namespace dotnetapp.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }

        public string? OrganizerId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        public TimeSpan EventTime { get; set; }

        [Required]
        public string Venue { get; set; } = string.Empty;

        [Range(1, 1000000)]
        public int Capacity { get; set; }

        public int BookedSeats { get; set; }

        public int RemainingSeats { get; set; }

        [Range(0, 10000000)]
        public decimal TicketPrice { get; set; }

        public string? EventImage { get; set; }

        public string Status { get; set; } = "Active";

        public string? OrganizerName { get; set; }

        public double AverageRating { get; set; }

        public int TotalReviews { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}