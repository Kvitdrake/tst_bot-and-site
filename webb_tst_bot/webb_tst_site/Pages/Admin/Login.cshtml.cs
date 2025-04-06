using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using webb_tst_site.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using webb_tst_site.Models;

namespace webb_tst_site.Pages.Admin
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoginModel> _logger;
        private readonly IAntiforgery _antiforgery;

        public LoginModel(AppDbContext context, ILogger<LoginModel> logger, IAntiforgery antiforgery)
        {
            _context = context;
            _logger = logger;
            _antiforgery = antiforgery;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "��� ������������ �����������")]
            public string Username { get; set; }

            [Required(ErrorMessage = "������ ����������")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet()
        {
            // ��������� CSRF-������
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewData["RequestVerificationToken"] = tokens.RequestToken;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ��������� ����� �������� - ��������� ���� � ����� ������� ��� ������������ "admin"
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Input.Username);

            if (user == null && Input.Username == "admin")
            {
                // ������� ���������� ������ ���� �� ����������
                user = new User
                {
                    Username = "admin",
                    PasswordHash = "dummy", // �� ������������ � ���� ������
                    Role = "Admin"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            if (user == null || user.Role != "Admin")
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // ���������� �������� ������
            var claims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.Role)
    };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                });

            return RedirectToPage("/Admin/Index");
        }
    }
}