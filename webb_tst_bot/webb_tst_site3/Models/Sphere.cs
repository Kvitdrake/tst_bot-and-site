using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class Sphere
    {
        public Sphere()
        {
            RuneDescriptions = new HashSet<RuneSphereDescription>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RuneSphereDescription> RuneDescriptions { get; set; }
    }
}