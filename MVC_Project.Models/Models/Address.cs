using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string? Street { get; set; }

        [Required, StringLength(50)]
        public string? City { get; set; }

        [Required, StringLength(50)]
        public string? State { get; set; }

        [Required, StringLength(10)]
        public string? ZipCode { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
