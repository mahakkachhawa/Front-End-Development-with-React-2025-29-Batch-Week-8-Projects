using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnetapp.Constants;

namespace dotnetapp.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        public int EventId { get; set; }

        [Range(1, 1000)]
        public int TicketQuantity { get; set; }

        [Range(0, 100000000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = BookingStatuses.Pending;

        [Required]
        public string PaymentStatus { get; set; } = PaymentStatuses.Pending;

        [Required]
        [StringLength(100)]
        public string BookingReference { get; set; } =
            Guid.NewGuid().ToString("N");

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser? Customer { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }
    }
}