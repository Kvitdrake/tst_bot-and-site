using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site.Data;
using webb_tst_site.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webb_tst_site.Pages.Admin.Runes
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(AppDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Rune Rune { get; set; }

        [BindProperty]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string ImageUrl { get; set; }

        public List<Sphere> AllSpheres { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                AllSpheres = await _context.Spheres.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading spheres");
                ModelState.AddModelError("", "Error loading spheres data");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // ��������� ����� ��� ���������� ����������� ����� ��� ������
                AllSpheres = await _context.Spheres.ToListAsync();

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid: {@Errors}",
                        ModelState.Values.SelectMany(v => v.Errors));
                    return Page();
                }

                // ������������� URL �����������
                Rune.ImageUrl = ImageUrl;

                // �������� ����������
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // ��������� ����
                    _context.Runes.Add(Rune);
                    await _context.SaveChangesAsync();

                    // ��������� �������� ��� ���� ����
                    foreach (var sphere in AllSpheres)
                    {
                        var description = Request.Form[$"SphereDescriptions[{sphere.Id}]"];

                        _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                        {
                            RuneId = Rune.Id,
                            SphereId = sphere.Id,
                            Description = description
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Rune created successfully: {RuneName}", Rune.Name);
                    return RedirectToPage("./Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating rune");
                    ModelState.AddModelError("", "Error saving rune to database");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in rune creation");
                ModelState.AddModelError("", "An unexpected error occurred");
                return Page();
            }
        }
    }
}