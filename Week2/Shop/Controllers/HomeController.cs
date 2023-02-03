using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shop.Data;
using Microsoft.EntityFrameworkCore;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly int _adminAccess = 1;

        public HomeController(ShopDbContext storeDbContext)
        {
            _context = storeDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var claimUser = HttpContext.User;
            var user = await _context.User.FirstOrDefaultAsync(user => 
                user.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            ViewBag.isAdmin = user?.Access == _adminAccess;

            ViewData["isLogged"] = claimUser.Identity!.IsAuthenticated;

            var categories = await _context.Category.ToListAsync();

            return View(categories);
        }

        public IActionResult Basket()
        {
            var claimUser = HttpContext.User;

            ViewData["isLogged"] = claimUser.Identity!.IsAuthenticated;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}