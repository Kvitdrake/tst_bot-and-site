using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Runes
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rune Rune { get; set; } = new Rune();

        [BindProperty]
        public Dictionary<int, string> SphereDescriptions { get; set; } = new();

        public List<Sphere> AllSpheres { get; set; }

        public async Task OnGetAsync()
        {
            AllSpheres = await _context.Spheres.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllSpheres = await _context.Spheres.ToListAsync();
                return Page();
            }

            Rune.CreatedAt = DateTime.UtcNow;
            Rune.UpdatedAt = DateTime.UtcNow;
            Rune.ImageUrl ??= "/images/default-rune.png";

            _context.Runes.Add(Rune);
            await _context.SaveChangesAsync();

            // Добавляем описания для каждой сферы
            foreach (var (sphereId, description) in SphereDescriptions)
            {
                _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                {
                    RuneId = Rune.Id,
                    SphereId = sphereId,
                    Description = description
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}