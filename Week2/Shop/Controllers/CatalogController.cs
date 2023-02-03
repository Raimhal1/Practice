using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ShopDbContext _context;

        public CatalogController(ShopDbContext storeDbContext)
        {
            _context = storeDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var claimUser = HttpContext.User;
            ViewData["isLogged"] = claimUser.Identity!.IsAuthenticated;
            var products = await _context.Product.ToListAsync();
            return View(products);
        }
    }
}