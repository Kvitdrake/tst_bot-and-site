using System.ComponentModel.DataAnnotations;

namespace webb_tst_site.Models
{
    public class Rune
    {
        public Rune()
        {
            SphereDescriptions = new HashSet<RuneSphereDescription>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string BaseDescription { get; set; }

        public string ImageUrl { get; set; } // Убрали [Required]

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RuneSphereDescription> SphereDescriptions { get; set; }
    }
}