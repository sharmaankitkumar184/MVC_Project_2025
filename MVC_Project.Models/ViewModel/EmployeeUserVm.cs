using MVC_Project.Models.Enums;
using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.ViewModel
{
    public class EmployeeUserVm
    {
        // Employee fields
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string? Name { get; set; }

        [Required, StringLength(10)]
        public string? EmployeeCode { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required, StringLength(15)]
        public string? Phone { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public Designation Designation { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public int? AddressId { get; set; }

        public int? ManagerId { get; set; }
        

        // User fields
        public UserRole Role { get; set; }

        public Address address { get; set; }
    }

}
