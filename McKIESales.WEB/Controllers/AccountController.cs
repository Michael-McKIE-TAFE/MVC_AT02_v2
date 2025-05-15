using System.Security.Claims;
using McKIESales.WEB.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace McKIESales.WEB.Controllers {
    public class AccountController : Controller {
        private readonly MongoDbContext _context;
        
        public AccountController (MongoDbContext context){
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Register() => View();

        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register (RegisterViewModel model){
            if (ModelState.IsValid){
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var user = new User {
                    Username = model.Username,
                    Email = model.Email,
                    Password = hashedPassword
                };

                _context.Users.InsertOneAsync(user);
                return RedirectToAction("Success");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login (LoginViewModel model){
            if (ModelState.IsValid){
                var user = _context.Users.Find(u => u.Email == model.Email).FirstOrDefault();

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password)){
                    var Claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principle = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);

                    return RedirectToAction("Dashboard", "Account");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }

        public IActionResult Success() => View();

        public async Task<IActionResult> Logout (){
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult Dashboard (){
            var username = User.Identity?.Name;
            return View(model: username);
        }
    }
}