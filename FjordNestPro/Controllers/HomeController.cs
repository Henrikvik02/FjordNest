using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FjordNestPro.Models;
using Microsoft.AspNetCore.Identity;

namespace FjordNestPro.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
        {
            ViewData["IsAdmin"] = true;

            ViewData["IsAdminPath"] = HttpContext?.Request?.Path.Value?.ToLower().StartsWith("/admin") ?? false;
        }
        else
        {
            ViewData["IsAdmin"] = false;
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

