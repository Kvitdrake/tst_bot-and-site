﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly Random _random = new();

        public RunesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomRune([FromQuery] int? sphereId = null)
        {
            var query = _context.Runes
                .Include(r => r.SphereDescriptions)
                .ThenInclude(sd => sd.Sphere)
                .AsQueryable();

            if (sphereId.HasValue)
            {
                query = query.Where(r => r.SphereDescriptions.Any(sd => sd.SphereId == sphereId.Value));
            }

            var runes = await query.ToListAsync();
            if (!runes.Any()) return NotFound();

            var rune = runes[_random.Next(runes.Count)];
            var description = rune.BaseDescription;
            var sphereName = "";

            if (sphereId.HasValue)
            {
                var sphereDesc = rune.SphereDescriptions.FirstOrDefault(sd => sd.SphereId == sphereId.Value);
                description = sphereDesc?.Description ?? description;
                sphereName = sphereDesc?.Sphere?.Name ?? "";
            }

            return Ok(new
            {
                name = rune.Name,
                imageUrl = string.IsNullOrEmpty(rune.ImageUrl) ? "/images/default-rune.png" : rune.ImageUrl,
                description,
                sphereName
            });
        }
    }
}