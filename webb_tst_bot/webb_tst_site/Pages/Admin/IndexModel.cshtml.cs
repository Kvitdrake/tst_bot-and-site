using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Models;
using webb_tst_site.Pages;

namespace webb_tst_site.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Sphere> Spheres { get; set; } // Добавлено свойство для сфер

        public async Task OnGetAsync()
        {
            Spheres = await _context.Spheres.ToListAsync(); // Загружаем сферы
        }
    }
}