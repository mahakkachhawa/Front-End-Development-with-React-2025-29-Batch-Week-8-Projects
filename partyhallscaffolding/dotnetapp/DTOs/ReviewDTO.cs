using System.ComponentModel.DataAnnotations;

namespace dotnetapp.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        public string? CustomerName { get; set; }

        [Required]
        public int EventId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}