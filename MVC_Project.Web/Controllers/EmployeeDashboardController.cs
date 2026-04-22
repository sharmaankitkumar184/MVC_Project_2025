using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories.IRepository;
using MVC_Project.Services.Data;
using System.Security.Claims;

namespace MVC_Project.Web.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeDashboardController : Controller
    {

        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeeRepository _emprepo;
        private readonly IDepartmentRepository _deptrepo;
        private readonly IAddressRepository _addrepo;
        private readonly IUserRepository _userRepo;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        public EmployeeDashboardController(ILogger<EmployeesController> logger, IEmployeeRepository emprepo, IDepartmentRepository deptrepo, IAddressRepository addrepo, IUserRepository userrepo, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _logger = logger;
            _emprepo = emprepo;
            _deptrepo = deptrepo;
            _addrepo = addrepo;
            _userRepo = userrepo;
            _env = env;
        }


        public async Task<IActionResult> MyProfile()
        {
            int userId = GetLoggedInUserId();

            var employeeDetails = await _emprepo.GetEmployeeByUserId(userId);
            var userDetails = await _userRepo.GetUserDetailsByUserIdAsync(userId);

            if (employeeDetails == null || userDetails ==null)
                return NotFound();

            var model = new EmployeeProfileViewModel
            {
                // ensure profile image path is set so view shows uploaded image
                ProfileImagePath = string.IsNullOrEmpty(employeeDetails.ProfileImagePath)
                    ? "/images/default-profile.png"
                    : employeeDetails.ProfileImagePath,
                FullName = employeeDetails.Name,
                Email = employeeDetails.Email,
                Username = userDetails.Username,
                Role = userDetails.Role.ToString(),
                DateOfRegister = userDetails.DateOfRegister,
                PhoneNumber = userDetails.PhoneNumber,
                Designation = employeeDetails.Designation.ToString(),
                EmployeeCode= employeeDetails.EmployeeCode,
                Gender = employeeDetails.Gender.ToString(),
                DateOfBirth =employeeDetails.DateOfBirth,
                DepartmentName =employeeDetails.Department.Name,
                ProjectName = "FMG",
                ManagerName ="Manager",
                Address = (userDetails.Address.Street + "," + userDetails.Address.City + "," + userDetails.Address.State).ToString(),
                BloodGroup=employeeDetails.BloodGroup,
                TimeZone=employeeDetails.TimeZone,
                CompanyName=employeeDetails.CompanyName,
                //Activities=employeeDetails?Activities,


            };

            return View(model);
        }
        private int GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            return int.Parse(userIdClaim.Value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile ProfileImageFile)
        {
            if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                int userId = GetLoggedInUserId();

                var employee = await _emprepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return NotFound();

                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "profile");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() +
                                        Path.GetExtension(ProfileImageFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(stream);
                }

                string relativePath = "/images/profile/" + uniqueFileName;
                var updated = await _emprepo.UpdateProfileImagePathByUserIdAsync(userId, relativePath);
                if (updated)
                {
                    // append timestamp to bust browser cache when redirecting
                    TempData["Success"] = "Profile image updated successfully.";
                    TempData["ProfileImageCacheBuster"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                }
                else
                {
                    TempData["Error"] = "Failed to update profile image.";
                }
                return RedirectToAction("MyProfile");
            }

            return RedirectToAction("MyProfile");
        }
    }

}
