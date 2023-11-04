using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;
using System.Threading.Tasks;
using FjordNestPro.ViewModels;

namespace FjordNestPro.Controllers
{
    public class ExploreController : Controller
    {
        private readonly FjordNestProDbContext _context;

        public ExploreController(FjordNestProDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string propertyType = null, int maxGuests = 0, decimal maxPricePerNight = 0)
        {
            var properties = _context.Properties
                .Include(p => p.Address)
                .AsQueryable();

            if (!string.IsNullOrEmpty(propertyType))
            {
                properties = properties.Where(p => p.PropertyType == propertyType);
            }

            if (maxGuests > 0)
            {
                properties = properties.Where(p => p.MaxGuests >= maxGuests);
            }

            if (maxPricePerNight > 0)
            {
                properties = properties.Where(p => p.PricePerNight <= maxPricePerNight);
            }

            var propertiesList = await properties.ToListAsync();

            return View(propertiesList);
        }
       

    }
}
