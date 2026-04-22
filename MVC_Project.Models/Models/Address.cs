using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public ICollection<UserData>? Users { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
