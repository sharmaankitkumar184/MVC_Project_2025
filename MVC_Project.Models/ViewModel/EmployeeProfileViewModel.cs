using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.ViewModel
{
    public class EmployeeProfileViewModel
    {
        // Account
        public string ProfileImagePath { get; set; }
        public IFormFile ProfileImageFile { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime DateOfRegister { get; set; }
        public string? PhoneNumber { get; set; }

        // Employee
        public string Designation { get; set; }
        public string EmployeeCode { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DepartmentName { get; set; }
        public string ProjectName { get; set; }
        public string ManagerName { get; set; }
        public string Address { get; set; }

        // Extra
        public string BloodGroup { get; set; }
        public string TimeZone { get; set; }
        public string CompanyName { get; set; }

        // Stats (for UI)
        public int Activities { get; set; }
    }
}
