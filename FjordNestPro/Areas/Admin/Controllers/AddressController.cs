using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;
using FjordNestPro.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace FjordNestPro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class AddressController : Controller
    {
        private readonly FjordNestProDbContext _context;
        private readonly ILogger<AddressController> _logger;

        public AddressController(FjordNestProDbContext context, ILogger<AddressController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string filterType = "All", string citySearchString = "", string postalCodeSearchString = "")
        {
            IQueryable<Address> query = _context.Addresses;

            if (!string.IsNullOrWhiteSpace(citySearchString))
            {
                query = query.Where(a => a.City.Contains(citySearchString));
            }

            if (!string.IsNullOrWhiteSpace(postalCodeSearchString))
            {
                query = query.Where(a => a.Postcode.Contains(postalCodeSearchString));
            }

            switch (filterType)
            {
                case "HasProperties":
                    query = query.Where(a => a.Properties != null && a.Properties.Any());
                    break;
                case "None":
                    query = query.Where(a => (a.Properties == null || !a.Properties.Any()));
                    break;
            }

            var addresses = await query.ToListAsync();

            var viewModel = new AddressIndexViewModel
            {
                Addresses = addresses,
                FilterType = filterType,
                FilterOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "All", Text = "All" },
                    new SelectListItem { Value = "HasProperties", Text = "Has Properties" },
                    new SelectListItem { Value = "None", Text = "None" }
                }
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Addresses == null)
            {
                _logger.LogWarning($"Address details for ID: {id} was not found.");
                return NotFound();
            }

            var address = await _context.Addresses.Include(a => a.Properties).FirstOrDefaultAsync(m => m.AddressID == id);

            if (address == null)
            {
                _logger.LogWarning($"Address with ID: {id} was not found.");
                return NotFound();
            }

            return View(address);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address address)
        {
            if (ModelState.IsValid)
            {
                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            _logger.LogError("Model state was invalid during the creation of an address.");
            return View(address);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Addresses == null)
            {
                _logger.LogWarning($"Attempted to edit an address with ID: {id}, but it wasn't found.");
                return NotFound();
            }

            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                _logger.LogWarning($"Address with ID: {id} was not found for editing.");
                return NotFound();
            }
            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Address address)
        {
            if (id != address.AddressID)
            {
                _logger.LogError($"Address ID mismatch during editing. Expected ID: {id}, but got ID: {address.AddressID} from model.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.AddressID))
                    {
                        _logger.LogError($"Concurrency exception occurred for address with ID: {address.AddressID}. Address not found.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogError($"Model state was invalid during the editing of address with ID: {address.AddressID}.");
            return View(address);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Addresses == null)
            {
                _logger.LogWarning($"Attempted to delete an address with ID: {id}, but it wasn't found.");
                return NotFound();
            }

            var address = await _context.Addresses.FirstOrDefaultAsync(m => m.AddressID == id);
            if (address == null)
            {
                _logger.LogWarning($"Address with ID: {id} was not found for deletion.");
                return NotFound();
            }
            return View(address);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Addresses == null)
            {
                _logger.LogError("Entity set 'FjordNestProDbContext.Addresses' is null.");
                return Problem("Entity set 'FjordNestProDbContext.Addresses' is null.");
            }

            var address = await _context.Addresses.FindAsync(id);

            if (address != null)
            {
                bool hasProperties = _context.Properties.Any(p => p.AddressID == id);

                if (hasProperties)
                {
                    _logger.LogWarning($"Attempted to delete address with ID: {id} which has associated properties.");
                    ViewData["ErrorMessage"] = "This address has associated properties and cannot be deleted.";
                    return View(address);
                }

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Attempted to delete address with ID: {id}, but it wasn't found.");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AddressExists(int id)
        {
            return (_context.Addresses?.Any(e => e.AddressID == id)).GetValueOrDefault();
        }
    }
}