
using System.Data;
using FjordNestPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FjordNestPro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class UserController : Controller
    {
        private readonly FjordNestProDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;  // <-- Logger declaration

        public UserController(UserManager<ApplicationUser> userManager, FjordNestProDbContext context, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;   // <-- Initialize logger
        }


        public async Task<IActionResult> Index(string idSearch, string userNameSearch, string emailSearch)
        {
            _logger.LogInformation("Fetching users based on search parameters.");
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(idSearch))
            {
                users = users.Where(u => !string.IsNullOrEmpty(u.Id) && u.Id.Contains(idSearch));
            }

            if (!string.IsNullOrEmpty(userNameSearch))
            {
                users = users.Where(u => !string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(userNameSearch));
            }

            if (!string.IsNullOrEmpty(emailSearch))
            {
                users = users.Where(u => !string.IsNullOrEmpty(u.Email) && u.Email.Contains(emailSearch));
            }

            return View(await users.ToListAsync());
        }

        public async Task<IActionResult> Edit(string id)
        {
            _logger.LogInformation("Fetching users based on search parameters.");
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,PhoneNumber")] ApplicationUser user)
        {
            _logger.LogInformation($"Attempting to edit user details. User ID: {id}");

            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var storedUser = await _userManager.FindByIdAsync(id);
                    if (storedUser == null)
                    {
                        return NotFound();
                    }

                    storedUser.UserName = user.UserName;
                    storedUser.Email = user.Email;
                    storedUser.PhoneNumber = user.PhoneNumber;

                    await _userManager.UpdateAsync(storedUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(user.Id))
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
            return View(user);
        }

        private async Task<bool> UserExists(string id)
        {
            _logger.LogInformation($"Fetching user details. User ID: {id}");
            return await _userManager.FindByIdAsync(id) != null;
        }

        public async Task<IActionResult> Details(string id)
        {
            _logger.LogInformation($"Fetching user details for deletion. User ID: {id}");
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .Include(u => u.Properties) 
                .Include(u => u.Bookings)   
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation($"Deleting user and related data. User ID: {id}");
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            bool hasRelatedData = _context.Bookings.Any(b => b.GuestID == id) ||
                                  _context.Properties.Any(p => p.OwnerID == id) ||
                                  _context.Reviews.Any(r => r.GuestID == id);

            if (hasRelatedData)
            {
                ViewBag.WarningMessage = "Denne brukeren har relaterte data (som Properties, Bookings, eller Reviews). Er du sikker på at du vil slette brukeren og all relatert data?";
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            _logger.LogInformation("Loading user creation view.");
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var bookingsToDelete = _context.Bookings.Where(b => b.GuestID == id);
                _context.Bookings.RemoveRange(bookingsToDelete);

                var propertiesToDelete = _context.Properties.Where(p => p.OwnerID == id);
                _context.Properties.RemoveRange(propertiesToDelete);

                var reviewsToDelete = _context.Reviews.Where(r => r.GuestID == id);
                _context.Reviews.RemoveRange(reviewsToDelete);

                await _context.SaveChangesAsync();

                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser model)
        {
            _logger.LogInformation("Attempting to create a new user.");
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name");
                    return View(model);
                }

                var userCreationResult = await _userManager.CreateAsync(model, model.Password);
                if (userCreationResult.Succeeded)
                {
                    foreach (var role in model.Roles)
                    {
                        var roleAssignmentResult = await _userManager.AddToRoleAsync(model, role.RoleId);
                        if (!roleAssignmentResult.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, $"Failed to assign role {role.RoleId} to user.");
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in userCreationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }
    }
}


