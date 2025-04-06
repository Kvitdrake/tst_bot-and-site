using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webb_tst_site.Data;
using webb_tst_site.Models;

namespace webb_tst_site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RunesController> _logger;

        public RunesController(AppDbContext context, ILogger<RunesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomRune([FromQuery] int? sphereId = null)
        {
            try
            {
                var random = new Random();
                IQueryable<Rune> runesQuery = _context.Runes
                    .Include(r => r.SphereDescriptions)
                    .ThenInclude(sd => sd.Sphere);

                if (sphereId.HasValue)
                {
                    runesQuery = runesQuery
                        .Where(r => r.SphereDescriptions.Any(sd => sd.SphereId == sphereId.Value));
                }

                var availableRunes = await runesQuery.ToListAsync();

                if (availableRunes.Count == 0)
                {
                    return NotFound(sphereId.HasValue
                        ? "No runes available for selected sphere"
                        : "No runes available");
                }

                var randomRune = availableRunes[random.Next(availableRunes.Count)];
                string description = randomRune.BaseDescription;
                string sphereName = null;

                if (sphereId.HasValue)
                {
                    var sphereDescription = randomRune.SphereDescriptions
                        .FirstOrDefault(sd => sd.SphereId == sphereId.Value);

                    if (sphereDescription != null)
                    {
                        description = sphereDescription.Description;
                        sphereName = sphereDescription.Sphere?.Name;
                    }
                }
                else if (randomRune.SphereDescriptions.Count > 0)
                {
                    var randomDescription = randomRune.SphereDescriptions
                        .OrderBy(x => random.Next())
                        .First();

                    description = randomDescription.Description;
                    sphereName = randomDescription.Sphere?.Name;
                }

                return Ok(new
                {
                    name = randomRune.Name,
                    imageUrl = !string.IsNullOrEmpty(randomRune.ImageUrl)
                        ? randomRune.ImageUrl
                        : "/images/default-rune.png",
                    description = description,
                    sphereName = sphereName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating random rune");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
