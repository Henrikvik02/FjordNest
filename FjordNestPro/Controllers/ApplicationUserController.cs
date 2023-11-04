using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FjordNestPro.Models;

namespace FjordNestPro.Controllers
{
    public class ApplicationUserController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }


    }
}
