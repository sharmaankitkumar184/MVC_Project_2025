using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories;
using MVC_Project.Services.Repositories.IRepository;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MVC_Project.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AccountController(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }
        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _userRepo.IsEmailRegisteredAsync(model.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var newUser = new UserData
            {
                FullName = model.FullName,
                Email = model.Email,
                Username = model.Username, // or model.Email.Split('@')[0] if you're not using input
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Password = ComputeSha256Hash(model.Password),
                DateOfRegister = DateTime.Now
            };


            bool result = await _userRepo.RegisterUserAsync(newUser);
            if (result)
                return RedirectToAction("Login");

            ModelState.AddModelError("", "Registration failed. Try again.");
            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userRepo.AuthenticateUserAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(2)
            });

            return RedirectToAction("Index", "Employees");
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

    }
}
