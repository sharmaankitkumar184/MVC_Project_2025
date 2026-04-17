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

        [Required(ErrorMessage = "Base salary is required")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999.99, ErrorMessage = "Base salary must be a valid amount")]
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999.99, ErrorMessage = "Bonus must be a valid amount")]
        public decimal Bonus { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999.99, ErrorMessage = "Deductions must be a valid amount")]
        public decimal Deductions { get; set; } = 0;

        [NotMapped]
        public decimal NetSalary => BaseSalary + Bonus - Deductions;

        [NotMapped]
        public decimal GrossSalary => BaseSalary + Bonus;

        [Required(ErrorMessage = "Employee code is required")]
        public string? EmployeeCode { get; set; }

        [ForeignKey("EmployeeCode")]
        public Employee? Employee { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
