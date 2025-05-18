using System.Security.Claims;
using McKIESales.WEB.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace McKIESales.WEB.Controllers {
    /// <summary>
    /// The `AccountController` manages user registration, login, and authentication. 
    /// It allows anonymous access to registration and login views. During registration, 
    /// it hashes the user's password using BCrypt and saves the user to MongoDB. The 
    /// login action verifies the user's credentials, signs them in using cookies, and 
    /// redirects to the dashboard. If login fails, an error is added to the model. The 
    /// `Dashboard` action is protected with the `[Authorize]` attribute, showing the 
    /// logged-in user's name. The controller also includes actions for logging out and a 
    /// success page after successful registration.
    /// </summary>
    public class AccountController : Controller {
        private readonly MongoDbContext _context;
        
        public AccountController (MongoDbContext context){
            _context = context;
        }

        //  Returns the registration view. It is marked with the
        //  `[AllowAnonymous]` attribute, meaning it can be accessed
        //  by users who are not logged in.
        [AllowAnonymous]
        public IActionResult Register() => View();

        //  Returns the login view. It is marked with the
        //  `[AllowAnonymous]` attribute, allowing users who are not logged in
        //  to access the login page where they can input their credentials.
        [AllowAnonymous]
        public IActionResult Login() => View();

        //  This method handles the form submission for user registration.
        //  It checks if the model is valid, hashes the password using `BCrypt`,
        //  creates a new `User` object, and inserts it into the database. If
        //  successful, the user is redirected to the "Success" view. If the model
        //  is invalid, the registration form is returned with validation errors.
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

        //  This method is responsible for handling user login. It checks if the submitted
        //  form data is valid. If the model is valid, it attempts to find the user by email
        //  in the database. If the user is found and the provided password matches the hashed
        //  password stored in the database (using `BCrypt.Verify`), it creates claims for the
        //  user, signs them in using cookie-based authentication, and redirects them to the
        //  "Dashboard" view. If the login attempt is unsuccessful, an error message is added to
        //  the model, and the login form is returned with the validation errors.
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

        //  Returns a view, likely to inform the user that an operation (such as registration)
        //  was completed successfully. There's no additional logic in this method; it only
        //  serves to render the success page.
        public IActionResult Success() => View();

        //  This method is responsible for logging the user out. It uses `SignOutAsync` to sign
        //  the user out of the current session using cookie-based authentication. After the user
        //  is signed out, it redirects them to the "Login" action, effectively sending them to
        //  the login page.
        public async Task<IActionResult> Logout (){
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        //  This method is decorated with the `[Authorize]` attribute, meaning only authenticated
        //  users can access it. It retrieves the username of the currently logged-in user from
        //  the `User.Identity.Name` property and passes it to the view. The view then displays
        //  the username or potentially other user-specific data.
        [Authorize]
        public IActionResult Dashboard (){
            var username = User.Identity?.Name;
            return View(model: username);
        }
    }
}