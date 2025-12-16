using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models.Models
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateOfRegister { get; set; } = DateTime.Now;
    }
}
