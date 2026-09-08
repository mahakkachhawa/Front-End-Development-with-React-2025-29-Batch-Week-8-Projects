using System.ComponentModel.DataAnnotations;

namespace dotnetapp.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        public int EventId { get; set; }

        public string? EventName { get; set; }

        public DateTime? EventDate { get; set; }

        public TimeSpan? EventTime { get; set; }

        public string? Venue { get; set; }

        public decimal TicketPrice { get; set; }

        [Range(1, 1000)]
        public int TicketQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string BookingReference { get; set; } = string.Empty;
    }
}