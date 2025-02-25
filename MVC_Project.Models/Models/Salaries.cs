using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.Models
{
    public class Salaries
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonus { get; set; } = 0; // Default to zero

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deductions { get; set; } = 0; // Default to zero

        [NotMapped] // This is not stored in the database
        public decimal NetSalary => BaseSalary + Bonus - Deductions;

        [Required]
        public string? EmployeeCode { get; set; }  // EmployeeCode as FK

        [ForeignKey("EmployeeCode")]
        public Employee? Employee { get; set; } // Navigation property

    }

}
