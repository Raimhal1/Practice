using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models.Store;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Encodings;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Shop.Controllers
{
    public class UserController : Controller
    {

        private readonly ShopDbContext _shopDbContext;
        private static int _minPaswordLenght = 8;

        public UserController(ShopDbContext storeDbContext)
        {
            _shopDbContext = storeDbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var claimUser = HttpContext.User;

            if (claimUser.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password, bool KeepLoggedIn)
        {
            var messages = new List<string>();

            if (string.IsNullOrEmpty(Email))
                messages.Add("Email is required");

            if (string.IsNullOrEmpty(Password))
                messages.Add("Password is required");

            if (messages.Count > 0)
            {
                ViewBag.ValidateMessage = messages;
                return View();
            }

            var user = await _shopDbContext.User.FirstOrDefaultAsync(user => user.Email == Email);

            if (user == null)
            {
                messages.Add("Incorrect email or there is no account with this email");
                ViewBag.ValidateMessage = messages;
                return View();
            }

            if (user.Password != Convert.ToBase64String(Encoding.UTF8.GetBytes(Password)))
            {
                messages.Add("Incorrect password");
                ViewBag.ValidateMessage = messages;
                return View();
            }

            if (user.KeepLoggedIn != KeepLoggedIn)
            {
                user.KeepLoggedIn = KeepLoggedIn;
                _shopDbContext.SaveChanges();
            }

            await SignInAsync(user);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string confirmationPassword, bool keepLoggedIn)
        {
            var messages = new List<string>();

            if (string.IsNullOrEmpty(name))
                messages.Add("Name is rerquired");
            
            if (string.IsNullOrEmpty(email) || _shopDbContext.User.Any(user => user.Email == email))
                messages.Add("Email is incorrect or already in use");

            if (string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(confirmationPassword)
                || password.Length < _minPaswordLenght
                || password != confirmationPassword)
            {
                messages.Add("Password must be at least 8 characters or it doesn't match with confirmation password");
            }

            if (messages.Count > 0)
            {
                ViewBag.ValidateMessage = messages;
                return View();
            }

            var user = new User() { 
                Name = name,
                Email = email,
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password)),
                KeepLoggedIn = keepLoggedIn
            };

            await SignInAsync(user);

            _shopDbContext.User.Add(user);
            _shopDbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInAsync(User user)
        {
            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, user.Email)
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = user.KeepLoggedIn
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                properties
            );
        }
    }
}
