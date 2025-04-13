using System.ComponentModel.DataAnnotations;

namespace webb_tst_site.Models
{
    public class RuneSphereDescription
    {
        public int Id { get; set; }
        public int RuneId { get; set; }
        public Rune Rune { get; set; }
        public int SphereId { get; set; }
        public Sphere Sphere { get; set; }
        public string Description { get; set; } = string.Empty; // Значение по умолчанию
    }
}