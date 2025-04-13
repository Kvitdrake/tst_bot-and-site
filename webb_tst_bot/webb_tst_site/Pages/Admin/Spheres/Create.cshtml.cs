using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Data;
using webb_tst_site.Models;

namespace webb_tst_site.Pages.Admin.Spheres
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Sphere Sphere { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Sphere.ImageUrl ??= "/images/default-sphere.png";
            Sphere.CreatedAt = DateTime.UtcNow;
            Sphere.UpdatedAt = DateTime.UtcNow;

            _context.Spheres.Add(Sphere);
            await _context.SaveChangesAsync();

            // Добавляем связи со всеми рунами
            foreach (var rune in await _context.Runes.ToListAsync())
            {
                _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                {
                    RuneId = rune.Id,
                    SphereId = Sphere.Id,
                    Description = "Нет описания"
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}