using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using webb_tst_site.Data;
using webb_tst_site.Models;

namespace webb_tst_site.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(AppDbContext context, ILogger<LoginModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Требуется имя пользователя")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Требуется пароль")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                _logger.LogInformation($"Attempting login for user: {Input.Username}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Input.Username);
                if (user == null)
                {
                    _logger.LogWarning("User not found");
                    ModelState.AddModelError(string.Empty, "Неверные учетные данные");
                    return Page();
                }

                _logger.LogInformation($"Found user: {user.Username}, Role: {user.Role}");

                using var sha256 = SHA256.Create();
                var inputHash = Convert.ToHexString(
                    sha256.ComputeHash(Encoding.UTF8.GetBytes(Input.Password)));

                _logger.LogInformation($"Input password hash: {inputHash}");
                _logger.LogInformation($"Stored password hash: {user.PasswordHash}");

                if (user.PasswordHash != inputHash)
                {
                    _logger.LogWarning("Password mismatch");
                    ModelState.AddModelError(string.Empty, "Неверные учетные данные");
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                    });

                return RedirectToPage("/Admin/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе");
                ModelState.AddModelError(string.Empty, "Произошла ошибка при входе");
                return Page();
            }
        }
    }
}