using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webb_tst_site.Models;

namespace webb_tst_site.Pages.Admin.Runes
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Rune> Runes { get; set; }

        public async Task OnGetAsync()
        {
            Runes = await _context.Runes
                .Include(r => r.SphereDescriptions)
                .ThenInclude(sd => sd.Sphere)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var rune = await _context.Runes.FindAsync(id);
            if (rune != null)
            {
                _context.Runes.Remove(rune);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}