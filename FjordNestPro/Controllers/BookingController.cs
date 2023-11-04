using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FjordNestPro.Controllers
{
    public class BookingController : Controller
    {
        private readonly FjordNestProDbContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(FjordNestProDbContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching bookings with related user and property data");
            var fjordNestProDbContext = _context.Bookings.Include(b => b.User).Include(b => b.Property);
            return View(await fjordNestProDbContext.ToListAsync());
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogWarning("Attempted to access Booking/Details without providing a booking ID");

            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Property)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Authorize]
        // GET: Booking/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Navigating to Create Booking view");

            ViewData["PropertyID"] = new SelectList(_context.Properties, "PropertyID", "Description");
            ViewData["GuestID"] = new SelectList(_context.Users, "ID", "ID");
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,GuestID,PropertyID,CheckInDate,CheckOutDate")] Booking booking)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Creating a new booking for user {UserId}", currentUserId);

            Console.WriteLine($"Er her: {currentUserId}");
            if (string.IsNullOrEmpty(currentUserId))
            {
                // Håndter feilen her, for eksempel:
                ModelState.AddModelError(string.Empty, "Kunne ikke identifisere brukeren. Vennligst prøv på nytt.");
                return View(booking);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    booking.GuestID = currentUserId;
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

            }
            
            ViewData["PropertyID"] = new SelectList(_context.Properties, "PropertyID", "Description", booking.PropertyID);
            ViewData["GuestID"] = new SelectList(_context.Users, "ID", "ID", booking.GuestID);
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["GuestID"] = new SelectList(_context.Users, "Id", "Id", booking.GuestID);
            ViewData["PropertyID"] = new SelectList(_context.Properties, "PropertyID", "Description", booking.PropertyID);
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,GuestID,PropertyID,CheckInDate,CheckOutDate")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
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
            ViewData["GuestID"] = new SelectList(_context.Users, "Id", "Id", booking.GuestID);
            ViewData["PropertyID"] = new SelectList(_context.Properties, "PropertyID", "Description", booking.PropertyID);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Property)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'FjordNestProDbContext.Bookings'  is null.");
            }
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
          return (_context.Bookings?.Any(e => e.BookingID == id)).GetValueOrDefault();
        }
    }
}
