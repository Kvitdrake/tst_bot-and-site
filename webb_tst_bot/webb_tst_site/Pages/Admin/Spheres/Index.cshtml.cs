using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Models;
using Microsoft.AspNetCore.Mvc;

namespace webb_tst_site.Pages.Admin.Spheres
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Sphere> Spheres { get; set; } // Добавлено свойство Spheres

        public async Task OnGetAsync()
        {
            Spheres = await _context.Spheres.ToListAsync(); // Загрузка списка сфер
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var sphere = await _context.Spheres.FindAsync(id);
            if (sphere != null)
            {
                _context.Spheres.Remove(sphere);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}