using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Models;

namespace webb_tst_site.Pages
{
    public class RandomizerModel : PageModel
    {
        private readonly AppDbContext _context;

        public RandomizerModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Sphere> Spheres { get; set; }

        public async Task OnGetAsync()
        {
            Spheres = await _context.Spheres
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}