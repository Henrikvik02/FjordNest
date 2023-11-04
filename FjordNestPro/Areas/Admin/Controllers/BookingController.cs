using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace FjordNestPro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class BookingController : Controller
    {
        private readonly FjordNestProDbContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(FjordNestProDbContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string userId, int? propertyId, DateTime? checkInDate, DateTime? checkOutDate)
        {
            _logger.LogInformation("Fetching bookings based on filters.");

            var query = _context.Bookings
                                .Include(b => b.Property)
                                .Include(b => b.User)
                                .Where(b => b.Property != null && b.User != null)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(b => b.GuestID == userId);
            }

            if (propertyId.HasValue)
            {
                query = query.Where(b => b.PropertyID == propertyId.Value);
            }

            if (checkInDate.HasValue)
            {
                query = query.Where(b => b.CheckInDate == checkInDate.Value);
            }

            if (checkOutDate.HasValue)
            {
                query = query.Where(b => b.CheckOutDate == checkOutDate.Value);
            }

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                _logger.LogWarning("Attempted to get booking details but ID is null or the context is missing.");
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID: {id} was not found.");
                return NotFound();
            }

            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                _logger.LogWarning("Attempted to delete a booking but ID is null or the context is missing.");
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID: {id} was not found for deletion.");
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bookings == null)
            {
                _logger.LogError("Entity set 'FjordNestProDbContext.Bookings' is null.");
                return Problem("Entity set 'FjordNestProDbContext.Bookings' is null.");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _logger.LogInformation($"Deleting booking with ID: {id}.");
                _context.Bookings.Remove(booking);
            }
            else
            {
                _logger.LogWarning($"Booking with ID: {id} not found during deletion.");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            var exists = (_context.Bookings?.Any(e => e.BookingID == id)).GetValueOrDefault();
            if (!exists)
            {
                _logger.LogWarning($"Booking with ID: {id} does not exist.");
            }
            return exists;
        }
    }
}
