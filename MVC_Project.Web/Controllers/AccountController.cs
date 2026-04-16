using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories;
using MVC_Project.Services.Repositories.IRepository;
using MVC_Project.Services.Services;
using MVC_Project.Services.Services.IService;
using NuGet.Common;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace MVC_Project.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AccountController(IUserRepository userRepo, IConfiguration config , IEmailService emailservice)
        {
            _userRepo = userRepo;
            _config = config;
            _emailService = emailservice;
        }
        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _userRepo.AuthenticateUserAsync(model.Email,model.Password);

            if (result.Result == LoginResult.EmailNotFound)
            {
                ModelState.AddModelError("Email", "This email address is not registered. Please check it or sign up.");
                return View(model);
            }

            if (result.Result == LoginResult.InvalidPassword)
            {
                ModelState.AddModelError("Password", "That password doesn’t match this account. Try again or reset your password.");
                return View(model);
            }

            // SUCCESS
            var user = result.User;


            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(2)
            });

            // After successful login
            if (user.Role == UserRole.Admin)
            {
                return RedirectToAction("Index", "Employees");
            }
            else if (user.Role == UserRole.Manager)
            {
                return RedirectToAction("MyTeam", "ManagerDashboard");
            }
            else
            {
                return RedirectToAction("MyProfile", "EmployeeDashboard");
            }

        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Step 1: Check if email exists
             var userexists=  await _userRepo.GetUserDetailsByEmailAsync(email);
            if (userexists==null)
            {
                return Json(new
                {
                    success = false,
                    message = "No account found with this Email."
                });
            }

            if (userexists!=null)
            {
                var token = GenerateResetToken();

                userexists.PasswordResetToken = token;
                userexists.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

                await _userRepo.UpdateAsync(userexists);
                var baseUrl = _config["AppSettings:BaseUrl"];

                var resetLink =
                          $"{baseUrl}/Account/ResetPassword" +
                          $"?token={WebUtility.UrlEncode(token)}" +
                          $"&email={WebUtility.UrlEncode(email)}";


                try
                {
                    await _emailService.SendPasswordResetEmail(email, resetLink);
                }
                catch (Exception ex)
                {
                    // TEMP: log the real error
                    throw new Exception("Email sending failed: " + ex.Message);
                }

            }

            // Always generic response (security)
            return Json(new
            {
                success = true,
                message = "If an account exists, a password reset link has been sent.",
                email = email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotEmail(string phoneNumber)
        {
            var user = _userRepo.GetUserDetailsByPhoneAsync(phoneNumber).Result;

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No account found with this phone number."
                });
            }

            // Mask email for security
            var email = user.Email;
            var maskedEmail = email.Substring(0, 3) + "****" + email.Substring(email.IndexOf("@"));

            return Json(new
            {
                success = true,
                message = $"Your registered email is {maskedEmail}"
            });
        }

        private string GenerateResetToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var user = await _userRepo.GetByResetTokenAsync(token, email);

            if (user == null)
            {
                Console.WriteLine("User NOT FOUND");
                return View("ResetLinkExpired");
            }

            if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                Console.WriteLine("Token EXPIRED");
                return View("ResetLinkExpired");
            }

            ViewBag.Token = token;
            ViewBag.Email = email;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword( string token,string email,string newPassword,string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                ViewBag.Token = token;
                ViewBag.Email = email;
                return View();
            }

            var user = await _userRepo.GetByResetTokenAsync(token, email);

            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return View("ResetLinkExpired");

            // ✅ Hash password with BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // ✅ Clear reset token
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _userRepo.UpdateAsync(user);

            return RedirectToAction("Login");
        }

    }
}
