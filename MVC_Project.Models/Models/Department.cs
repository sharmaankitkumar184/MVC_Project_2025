using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public ICollection<Project> Projects { get; set; } = new List<Project>();


    }
}
