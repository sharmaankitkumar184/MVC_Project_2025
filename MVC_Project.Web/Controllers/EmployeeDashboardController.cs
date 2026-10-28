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

            if (employeeDetails == null || userDetails == null)
                return NotFound();

            // Calculate experience years from CreatedAt
            int experienceYears = DateTime.Now.Year - employeeDetails.CreatedAt.Year;

            // Check if profile is complete
            bool isProfileComplete = !string.IsNullOrEmpty(employeeDetails.Name) &&
                                     !string.IsNullOrEmpty(employeeDetails.Email) &&
                                     !string.IsNullOrEmpty(employeeDetails.Phone) &&
                                     !string.IsNullOrEmpty(employeeDetails.ProfileImagePath) &&
                                     userDetails.Address != null;

            var model = new EmployeeProfileViewModel
            {
                // Profile Image
                ProfileImagePath = string.IsNullOrEmpty(employeeDetails.ProfileImagePath)
                    ? "/images/default-profile.png"
                    : employeeDetails.ProfileImagePath,

                // Account Information
                FullName = employeeDetails.Name,
                Email = employeeDetails.Email,
                Username = userDetails.Username,
                Role = userDetails.Role.ToString(),
                DateOfRegister = userDetails.DateOfRegister,
                PhoneNumber = userDetails.PhoneNumber,

                // Employee Information
                Designation = employeeDetails.Designation.ToString(),
                EmployeeCode = employeeDetails.EmployeeCode,
                Gender = employeeDetails.Gender.ToString(),
                DateOfBirth = employeeDetails.DateOfBirth,
                DepartmentName = employeeDetails.Department?.Name ?? "N/A",
                ProjectName = "FMG",
                ManagerName = employeeDetails.Manager?.Name ?? "Not Assigned",
                Address = $"{userDetails.Address?.Street ?? "N/A"}, {userDetails.Address?.City ?? "N/A"}, {userDetails.Address?.State ?? "N/A"}",

                // Additional Information
                BloodGroup = employeeDetails.BloodGroup ?? "N/A",
                TimeZone = employeeDetails.TimeZone ?? "UTC",
                CompanyName = employeeDetails.CompanyName ?? "N/A",

                // Enhanced Fields
                JoiningDate = employeeDetails.CreatedAt.ToShortDateString(),
                ExperienceYears = Math.Max(0, experienceYears),
                EmploymentStatus = "Active",
                SkillsSummary = "Technical Professional",
                TeamMembersCount = (employeeDetails.TeamMembers?.Count ?? 0).ToString(),
                ReportingManager = employeeDetails.Manager?.Name ?? "N/A",
                LastUpdated = DateTime.Now,
                IsProfileComplete = isProfileComplete,
                Activities = employeeDetails.Activities ?? 0
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

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            int userId = GetLoggedInUserId();
            var employeeDetails = await _emprepo.GetEmployeeByUserId(userId);
            var userDetails = await _userRepo.GetUserDetailsByUserIdAsync(userId);

            if (employeeDetails == null || userDetails == null)
                return NotFound();

            var model = new EmployeeProfileViewModel
            {
                ProfileImagePath = string.IsNullOrEmpty(employeeDetails.ProfileImagePath)
                    ? "/images/default-profile.png"
                    : employeeDetails.ProfileImagePath,
                FullName = employeeDetails.Name,
                Email = employeeDetails.Email,
                Username = userDetails.Username,
                PhoneNumber = userDetails.PhoneNumber,
                Address = $"{userDetails.Address?.Street ?? ""}, {userDetails.Address?.City ?? ""}, {userDetails.Address?.State ?? ""}",
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadProfileData()
        {
            int userId = GetLoggedInUserId();
            var employeeDetails = await _emprepo.GetEmployeeByUserId(userId);
            var userDetails = await _userRepo.GetUserDetailsByUserIdAsync(userId);

            if (employeeDetails == null || userDetails == null)
                return NotFound();

            var profileData = new
            {
                employeeDetails.Name,
                employeeDetails.Email,
                employeeDetails.EmployeeCode,
                userDetails.Username,
                employeeDetails.Designation,
                Department = employeeDetails.Department?.Name,
                ExportDate = DateTime.Now
            };

            var json = System.Text.Json.JsonSerializer.Serialize(profileData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

            return File(data, "application/json", $"Profile_{employeeDetails.EmployeeCode}.json");
        }

        [HttpGet]
        public IActionResult GetProfileStats()
        {
            // This can be expanded to fetch real statistics
            var stats = new
            {
                totalActivities = 12,
                projectsAssigned = 3,
                tasksCompleted = 48,
                performanceRating = 4.5
            };
            return Json(stats);
        }
    }
}
