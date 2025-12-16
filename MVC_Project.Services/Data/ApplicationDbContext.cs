using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Salaries> Salary { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<UserData> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define Composite Key: EmployeeCode and Id
            modelBuilder.Entity<Employee>()
                .HasKey(e => new { e.Id, e.EmployeeCode });

            // Ensure EmployeeCode is unique using HasIndex() method
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeCode)
                .IsUnique(); // Enforces uniqueness

            modelBuilder.Entity<Employee>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();  // This ensures that Id is auto-incremented


            // Department-Employee Relationship (One-to-Many)
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Address-Employee Relationship (One-to-Many)
            modelBuilder.Entity<Address>()
                .HasMany(a => a.Employees)
                .WithOne(e => e.Address)
                .HasForeignKey(e => e.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee-Salary Relationship (One-to-One using EmployeeCode as FK)
            modelBuilder.Entity<Salaries>()
                 .HasOne(s => s.Employee)
                 .WithOne(e => e.Salary)
                 .HasForeignKey<Salaries>(s => s.EmployeeCode)  // Using EmployeeCode as FK
                 .HasPrincipalKey<Employee>(e => e.EmployeeCode) // Ensure EmployeeCode is used as principal key
                 .OnDelete(DeleteBehavior.Cascade); // Specify delete behavior (optional)

        }
    }
}
