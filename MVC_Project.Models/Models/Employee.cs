using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Project.Models.Models
{
    public class Employee
    {
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
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; } // Navigation Property

        public int? AddressId { get; set; }  // Nullable
        public Address? Address { get; set; } // Navigation Property

        // Navigation property for the related Salary
        [NotMapped]
        public virtual Salaries Salary { get; set; }

    }

    public enum Gender
    {
        Male=1,
        Female=2,
        Other=3
    }
}
