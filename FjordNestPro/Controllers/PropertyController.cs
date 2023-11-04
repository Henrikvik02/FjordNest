using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;
using System.Security.Claims;
using System.Reflection;
using FjordNestPro.ViewModels;
using Microsoft.AspNetCore.Authorization;


namespace FjordNestPro.Controllers
{
    public class PropertyController : Controller
    {
        private readonly FjordNestProDbContext _context;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(FjordNestProDbContext context,ILogger<PropertyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Property
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all properties with associated Address and User.");
            var fjordNestProDbContext = _context.Properties.Include(p => p.Address).Include(p => p.User);
            return View(await fjordNestProDbContext.ToListAsync());
        }


        // GET: Property/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Fetching property details for property with ID: {id}.");
            var property = await _context.Properties.Include(p => p.Bookings).ThenInclude(b => b.Review)
                                                    .FirstOrDefaultAsync(p => p.PropertyID == id);

            if (property == null)
            {
                return NotFound();
            }


            var userAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressID == property.AddressID);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == property.OwnerID);

            var viewModel = new PropertyDetailsViewModel
            {
                PropertyID = property.PropertyID,
                Title = property.Title,
                PropertyType = property.PropertyType,
                Description = property.Description,
                MaxGuests = property.MaxGuests,
                PricePerNight = property.PricePerNight,
                LongTermStay = property.LongTermStay,
                ImageUrl = property.ImageUrl,


                Address = new AddressViewModel
                {
                    StreetName = property.Address.StreetName,
                    City = property.Address.City,
                    Postcode = property.Address.Postcode
                },


                Reviews = property.Bookings?.Where(b => b.Review != null).Select(b => new ReviewViewModel

                {
                    ReviewID = b.Review.ReviewID,
                    Content = b.Review.Content,
                    Rating = b.Review.Rating,
                    GuestID = b.Review.GuestID,
                    BookingID = b.BookingID
                }).ToList() ?? new List<ReviewViewModel>()


        };

            return View(viewModel);
        }

        // GET: Property/Create
        [Authorize]
        public IActionResult Create()
        {

            ViewData["AddressID"] = new SelectList(_context.Addresses, "AddressID", "City");
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Property/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OwnerID,AddressID,Title,PropertyType,Description,MaxGuests,PricePerNight,LongTermStay,ImageUrl")] Property property, IFormFile ImageUrl)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"Attempting to create a new property for user ID: {currentUserId}.");

            if (string.IsNullOrEmpty(currentUserId))
            {
                // Håndter feilen her, for eksempel:
                ModelState.AddModelError(string.Empty, "Kunne ikke identifisere brukeren. Vennligst prøv på nytt.");
                ViewData["AddressID"] = new SelectList(_context.Addresses, "AddressID", "City", property.AddressID);
                return View(property);
            }
            else
            {
                Console.WriteLine($"Skal sjekke validstate");
                if (ModelState.IsValid)
                {

                    if (ImageUrl != null && ImageUrl.Length > 0)
                    {
                        Console.WriteLine($"Skal sjekke imagefile");
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        Directory.CreateDirectory(uploadPath);

                        var filePath = Path.Combine(uploadPath, ImageUrl.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(fileStream);
                        }

                        Console.WriteLine($"Skal adde i database");
                        property.ImageUrl = "uploads/" + ImageUrl.FileName;
                        property.OwnerID = currentUserId;
                        _context.Add(property);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            Console.WriteLine($"Er på slutten");
            ViewData["AddressID"] = new SelectList(_context.Addresses, "AddressID", "City", property.AddressID);
            return View(property);
        }





        // GET: Property/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Properties == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            ViewData["AddressID"] = new SelectList(_context.Addresses, "AddressID", "City", @property.AddressID);
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", @property.OwnerID);
            return View(@property);
        }

        // POST: Property/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyID,OwnerID,AddressID,Title,PropertyType,Description,MaxGuests,PricePerNight,LongTermStay,ImageUrl")] Property @property)
        {
            _logger.LogInformation($"Attempting to edit property with ID: {id}.");

            if (id != @property.PropertyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.PropertyID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressID"] = new SelectList(_context.Addresses, "AddressID", "City", @property.AddressID);
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", @property.OwnerID);
            return View(@property);
        }

        // GET: Property/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Properties == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(p => p.Address)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PropertyID == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Property/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Attempting to delete property with ID: {id}.");

            if (_context.Properties == null)
            {
                return Problem("Entity set 'FjordNestProDbContext.Properties'  is null.");
            }
            var @property = await _context.Properties.FindAsync(id);
            if (@property != null)
            {
                _context.Properties.Remove(@property);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {

          return (_context.Properties?.Any(e => e.PropertyID == id)).GetValueOrDefault();
        }
    }
}
