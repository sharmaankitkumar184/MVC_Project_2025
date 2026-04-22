using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public int? AddressId { get; set; }  // Nullable
        public Address? Address { get; set; } // Navigation Property

        [DataType(DataType.DateTime)]
        public DateTime DateOfRegister { get; set; } = DateTime.Now;

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public UserRole Role { get; set; }

        // navigation
        public Employee Employee { get; set; } = null!;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
    public enum LoginResult
    {
        Success,
        EmailNotFound,
        InvalidPassword
    }
    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        Employee = 3
    }

}
