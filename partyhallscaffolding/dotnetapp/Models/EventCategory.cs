using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class EventCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Event> Events { get; set; } =
            new List<Event>();
    }
}