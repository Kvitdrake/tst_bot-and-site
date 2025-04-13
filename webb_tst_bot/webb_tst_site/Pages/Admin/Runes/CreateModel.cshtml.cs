using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Data;
using webb_tst_site.Models;

namespace webb_tst_site.Pages.Admin.Runes
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rune Rune { get; set; }

        public List<Sphere> AllSpheres { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AllSpheres = await _context.Spheres.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllSpheres = await _context.Spheres.ToListAsync();
                return Page();
            }

            Rune.ImageUrl ??= "/images/default-rune.png";
            Rune.CreatedAt = DateTime.UtcNow;
            Rune.UpdatedAt = DateTime.UtcNow;

            _context.Runes.Add(Rune);
            await _context.SaveChangesAsync();

            // Добавляем связи со всеми сферами
            foreach (var sphere in await _context.Spheres.ToListAsync())
            {
                _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                {
                    RuneId = Rune.Id,
                    SphereId = sphere.Id,
                    Description = "Нет описания"
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}