using System.ComponentModel.DataAnnotations;

namespace webb_tst_site.Models
{
    public class Sphere
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RuneSphereDescription> RuneDescriptions { get; set; }
    }
}
