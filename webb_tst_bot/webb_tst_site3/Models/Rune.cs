using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class Rune
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BaseDescription { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Связь с описаниями по сферам
        public List<RuneSphereDescription> SphereDescriptions { get; set; } = new();


        public int Order { get; set; } = 0;
    }
}