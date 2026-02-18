using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Project.Models.Models
{
    public class Employee
    {
        // ========================
        // BASIC EMPLOYEE INFO
        // ========================
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

        // 🔥 NEW FIELD
        [Required]
        public Designation Designation { get; set; }

        // ========================
        // ORGANIZATION STRUCTURE
        // ========================

        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; } // Navigation Property

        public int? AddressId { get; set; }  // Nullable
        public Address? Address { get; set; } // Navigation Property

        // Navigation property for the related Salary
        [NotMapped]
        public virtual Salaries Salary { get; set; }

        // ✅ SELF-REFERENCING MANAGER RELATION
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        public ICollection<Employee> TeamMembers { get; set; }
            = new List<Employee>();

        public int? UserId { get; set; }  // Nullable
        // navigation
        public UserData? User { get; set; }

    }
  
}
