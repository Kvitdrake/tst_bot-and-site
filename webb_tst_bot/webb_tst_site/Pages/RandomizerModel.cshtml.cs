using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Data;
using webb_tst_site.Models;

namespace webb_tst_site.Pages
{
    public class RandomizerModel : PageModel
    {
        private readonly AppDbContext _context;
        public List<Sphere> Spheres { get; set; }

        public RandomizerModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Spheres = await _context.Spheres
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}