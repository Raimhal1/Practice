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
    public class ProductController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly int _adminAccess;

        public ProductController(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var claimUser = HttpContext.User;
            var user = await _context.User.FirstOrDefaultAsync(u =>
                u.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            if (user == null || user.Access < _adminAccess)
                return RedirectToAction("Index", "Home");

            var products = await _context.Product
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> View(int? id)
        {
            var claimUser = HttpContext.User;
            ViewData["isLogged"] = claimUser.Identity!.IsAuthenticated;

            var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null)
                return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var claimUser = HttpContext.User;
            var user = await _context.User.FirstOrDefaultAsync(u => 
                u.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            if (user == null || user.Access < _adminAccess)
                return RedirectToAction("Index", "Home");

            var categories = await _context.Category.ToListAsync();

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Color,Sizes,Description,CategoryId")] Product product, IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return View(product);

            var prefix = @"wwwroot\img\product";
            var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
            var filePath = Path.Combine(prefix, fileName);

            using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);

            product.ImgName = fileName;

            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var claimUser = HttpContext.User;
            var user = await _context.User.FirstOrDefaultAsync(u => 
                u.Email == claimUser.FindFirstValue(ClaimTypes.Email));

            if (user == null || user.Access < _adminAccess)
                return RedirectToAction("Index", "Home");

            var product = await _context.Product.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound();

            var categories = await _context.Category.ToListAsync();

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Color,Sizes,Description,CategoryId,ImgName")] Product product, IFormFile file)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                var prefix = @"wwwroot\img\product";
                var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
                var filePath = Path.Combine(prefix, fileName);

                var oldImage = Path.Combine(prefix, product.ImgName);

                using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);

                if (System.IO.File.Exists(oldImage))
                    System.IO.File.Delete(oldImage);

                product.ImgName = fileName;
            }

            _context.Product.Update(product);
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

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound();

            var prefix = @"wwwroot\img\product";
            var oldImage = Path.Combine(prefix, product.ImgName);


            if (System.IO.File.Exists(oldImage))
                System.IO.File.Delete(oldImage);

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
