using McKIESales.WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace McKIESales.WEB.Controllers {
    public class AccountController : Controller {
        private readonly MongoDbContext _context;
        
        public AccountController (MongoDbContext context){
            _context = context;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register (RegisterViewModel model){
            if (ModelState.IsValid){
                var user = new User {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password
                };

                _context.Users.InsertOneAsync(user);
                return RedirectToAction("Success");
            }

            return View(model);
        }

        public IActionResult Success() => View();
    }
}