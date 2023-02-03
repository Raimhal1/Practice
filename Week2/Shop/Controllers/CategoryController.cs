using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models.Store;

namespace Shop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly int _adminAccess = 1;

        public CategoryController(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var claimUser = HttpContext.User;
            var user = await _context.User.FirstOrDefaultAsync(user => 
                user.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            if (user == null || user.Access < _adminAccess)
                return RedirectToAction("Index", "Home");

            var categories = await _context.Category.ToListAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            var claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(u => 
                u.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            if (user == null || user.Access < 1)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category, IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return View(category);

            var prefix = @"wwwroot\img\category";
            var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
            var filePath = Path.Combine(prefix, fileName);

            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            category.ImgName = fileName;

            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = await _context.User.FirstOrDefaultAsync(u => 
                u.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            if (user == null || user.Access < _adminAccess)
                return RedirectToAction("Index", "Home");

            var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImgName")] Category categoryModel, IFormFile file)
        {

            var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == categoryModel.Id);

            if (id != categoryModel.Id || category == null)
                return NotFound();

            if (file != null && file.Length > 0)
            {
                var prefix = @"wwwroot\img\category";
                var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
                var filePath = Path.Combine(prefix, fileName);

                var oldImg = Path.Combine(prefix, category.ImgName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                    if (System.IO.File.Exists(oldImg))
                    {
                        System.IO.File.Delete(oldImg);
                    }
                }

                category.ImgName = fileName;
            }

            _context.Category.Update(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var claimUser = HttpContext.User;
            var user = await _context.User.FirstOrDefaultAsync(u =>
                u.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            if (user == null || user.Access < _adminAccess)
                return RedirectToAction("Index", "Home");

            var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            var prefix = @"wwwroot\img\category";
            var oldImage = Path.Combine(prefix, category.ImgName);


            if (System.IO.File.Exists(oldImage))
                System.IO.File.Delete(oldImage);

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Products(int? id)
        {
            var category = await _context.Category
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            ViewBag.Category = category.Name;

            return View(category.Products);
        }
    }
}
