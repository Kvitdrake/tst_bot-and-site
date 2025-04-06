using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site.Models;

namespace webb_tst_site.Pages.Shared
{
    public class RandomizerModel : PageModel
    {

        public List<Sphere> Spheres { get; set; }
        public void OnGet()
        {
        }
    }
}
