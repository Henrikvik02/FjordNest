using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace FjordNestPro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class PropertyController : Controller
    {
        private readonly FjordNestProDbContext _context;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(FjordNestProDbContext context, ILogger<PropertyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string userId, string city, string propertyType, int? maxGuests, decimal? pricePerNight, bool? longTermStay)
        {
            _logger.LogInformation("Fetching properties based on filters.");

            var query = _context.Properties.Include(p => p.Address).Include(p => p.User).AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.User != null && p.User.Id == userId);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.Address != null && p.Address.City == city);
            }

            if (!string.IsNullOrEmpty(propertyType))
            {
                query = query.Where(p => p.PropertyType == propertyType);
            }

            if (maxGuests.HasValue)
            {
                query = query.Where(p => p.MaxGuests == maxGuests.Value);
            }

            if (pricePerNight.HasValue)
            {
                query = query.Where(p => p.PricePerNight == pricePerNight.Value);
            }

            if (longTermStay.HasValue)
            {
                query = query.Where(p => p.LongTermStay == longTermStay.Value);
            }

            return View(await query.ToListAsync());
        }
    }
}
