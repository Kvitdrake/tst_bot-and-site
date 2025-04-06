using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Data;
using webb_tst_site.Models;

namespace webb_tst_site.Pages.Admin.Runes
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rune Rune { get; set; }

        public List<Sphere> AllSpheres { get; set; }

        public Dictionary<int, string> SphereDescriptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                Rune = new Rune();
            }
            else
            {
                Rune = await _context.Runes
                    .Include(r => r.SphereDescriptions)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (Rune == null)
                {
                    return NotFound();
                }
            }

            AllSpheres = await _context.Spheres.ToListAsync();

            // Заполняем описания для сфер
            foreach (var sphere in AllSpheres)
            {
                var description = Rune.SphereDescriptions?
                    .FirstOrDefault(sd => sd.SphereId == sphere.Id)?
                    .Description ?? string.Empty;
                SphereDescriptions[sphere.Id] = description;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllSpheres = await _context.Spheres.ToListAsync();
                return Page();
            }

            if (Rune.Id == 0)
            {
                // Новая руна
                _context.Runes.Add(Rune);
                await _context.SaveChangesAsync();

                // Добавляем все сферы с пустыми описаниями
                foreach (var sphere in await _context.Spheres.ToListAsync())
                {
                    _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                    {
                        RuneId = Rune.Id,
                        SphereId = sphere.Id,
                        Description = Request.Form[$"SphereDescriptions[{sphere.Id}]"]
                    });
                }
            }
            else
            {
                // Обновление существующей руны
                var existingRune = await _context.Runes
                    .Include(r => r.SphereDescriptions)
                    .FirstOrDefaultAsync(r => r.Id == Rune.Id);

                if (existingRune == null)
                {
                    return NotFound();
                }

                existingRune.Name = Rune.Name;
                existingRune.BaseDescription = Rune.BaseDescription;
                existingRune.ImageUrl = Rune.ImageUrl;

                // Обновляем описания для сфер
                foreach (var description in existingRune.SphereDescriptions)
                {
                    description.Description = Request.Form[$"SphereDescriptions[{description.SphereId}]"];
                }

                // Добавляем новые сферы, если они появились
                var existingSphereIds = existingRune.SphereDescriptions.Select(sd => sd.SphereId);
                var allSphereIds = (await _context.Spheres.Select(s => s.Id).ToListAsync());

                foreach (var sphereId in allSphereIds.Except(existingSphereIds))
                {
                    _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                    {
                        RuneId = Rune.Id,
                        SphereId = sphereId,
                        Description = Request.Form[$"SphereDescriptions[{sphereId}]"]
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public string GetDescriptionForSphere(int sphereId)
        {
            return SphereDescriptions.TryGetValue(sphereId, out var description)
                ? description
                : string.Empty;
        }
    }
}