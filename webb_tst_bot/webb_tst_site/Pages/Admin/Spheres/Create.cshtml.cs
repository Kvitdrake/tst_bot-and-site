using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site.Data;
using webb_tst_site.Models;
using Microsoft.EntityFrameworkCore;

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

            // ≈сли ссылка на изображение не указана, устанавливаем пустую строку
            Sphere.ImageUrl ??= string.Empty;

            _context.Spheres.Add(Sphere);
            await _context.SaveChangesAsync();

            // ƒобавл€ем новую сферу ко всем существующим рунам
            var allRunes = await _context.Runes.ToListAsync();
            foreach (var rune in allRunes)
            {
                _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                {
                    RuneId = rune.Id,
                    SphereId = Sphere.Id,
                    Description = string.Empty
                });
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}