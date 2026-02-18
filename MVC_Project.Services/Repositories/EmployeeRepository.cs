using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MVC_Project.Services.Repositories
{
    public class EmployeeRepository :IEmployeeRepository
    {

        private readonly ApplicationDbContext _db;

        public EmployeeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IQueryable<Employee>> GetAllEmployee()
        {

            return await Task.FromResult(_db.Employees.Include(e => e.Department).Include(f=>f.Address).Include(s => s.Salary).AsQueryable());

        }

        public async Task<Employee?> GetEmployeeById(int? id)
        {
            if (id == 0 || id == null)
            {
                throw new ArgumentNullException(nameof(id), "EmployeeId cannot be null");
            }
            var employee = await _db.Employees.Include(s=>s.Salary).Include(s => s.Department).Include(s => s.Address).Include(r=>r.User)
           .FirstOrDefaultAsync(m => m.Id == id);
            return employee;
        }
        public async Task<EmployeeUserVm> AddEmployee(EmployeeUserVm employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            var nextEmployeeCode = await GenerateNextEmployeeCode();

            var rawName = employee.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rawName))
                throw new ArgumentException("Employee name is required");

            char firstInitial = char.ToLower(rawName[0]) ;

            int spaceIndex = rawName.IndexOf(' ');
            char secondInitial = (spaceIndex > 0 && spaceIndex + 1 < rawName.Length)
                ? char.ToLower(rawName[spaceIndex + 1])
                : firstInitial;
            // remove spaces
            var nameWithoutSpaces = rawName.Replace(" ", "").ToLower();

            // capitalize only first letter
            var formattedName = char.ToUpper(nameWithoutSpaces[0]) + nameWithoutSpaces.Substring(1);

            var Password = $"{formattedName}@184";

            string username = $"{firstInitial}{secondInitial}{nextEmployeeCode}".ToUpper();

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Create User
                var user = new UserData
                {
                    FullName = employee.Name,
                    Email = employee.Email,
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password),
                    PhoneNumber = employee.Phone,
                    AddressId = employee.AddressId,
                    Role = UserRole.Employee,
                    DateOfRegister = DateTime.Now
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync(); // generates user.Id
                                              // Create User
                var updatedemployee = new Employee
                {
                    

                    // 2️⃣ Update EMPLOYEE fields
                    EmployeeCode = nextEmployeeCode.ToString(),
                    Name = employee.Name,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    DateOfBirth = employee.DateOfBirth,
                    Gender = employee.Gender,
                    DepartmentId = employee.DepartmentId,
                    AddressId = employee.AddressId,
                    ManagerId = employee.ManagerId,
                    // Link employee to user
                    UserId = user.Id,
                    Designation = employee.Designation
                };
                _db.Employees.Add(updatedemployee);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return employee;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }


        public async Task<Employee> EditEmployee(int? id, EmployeeUserVm updatedEmployee,bool isAdmin)
        {
            if (updatedEmployee == null)
                throw new ArgumentNullException(nameof(updatedEmployee));

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Load existing employee
                var employee = await _db.Employees
                    .Include(e => e.User)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                    throw new KeyNotFoundException("Employee not found");

                // 2️⃣ Update EMPLOYEE fields
                employee.Name = updatedEmployee.Name;
                employee.Email = updatedEmployee.Email;
                employee.Phone = updatedEmployee.Phone;
                employee.DateOfBirth = updatedEmployee.DateOfBirth;
                employee.Gender = updatedEmployee.Gender;
                employee.Designation = updatedEmployee.Designation;
                employee.DepartmentId = updatedEmployee.DepartmentId;
                employee.AddressId = updatedEmployee.AddressId;


                // 3️⃣ Update USER fields (same logic as Create)
                if (employee.User != null)
                {
                    employee.User.FullName = updatedEmployee.Name ?? string.Empty;
                    employee.User.Email = updatedEmployee.Email ?? string.Empty;
                    employee.User.PhoneNumber = updatedEmployee.Phone ?? string.Empty;
                    employee.User.AddressId = updatedEmployee.AddressId;
                    if (isAdmin)
                    {
                        employee.User.Role = updatedEmployee.Role;
                    }

                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return employee;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<Employee> DeleteEmployee(int? id)
        {
            var employee = await _db.Employees.FindAsync(id);
            var user = await _db.Users.FirstOrDefaultAsync(n=>n.Email==employee.Email);
            if (employee != null)
            {
                _db.Employees.Remove(employee);
                _db.Users.Remove(user);
            }

            await _db.SaveChangesAsync();
            return employee;
        }
        public async Task<IQueryable<Employee?>> SearchEmployee(string empName)
        {
            if(empName == null)
            {
                throw new ArgumentNullException(nameof(empName), "Employee name cannot be null");
            }
            return await Task.FromResult(_db.Employees.Include(e => e.Department).Include(f => f.Address).Include(s => s.Salary)
                                .Where(e => e.Name.ToLower().Contains(empName.ToLower())).AsQueryable());

        }

        public async Task<int> GenerateNextEmployeeCode()
        {
            var lastCode = await _db.Employees
                .OrderByDescending(e => e.EmployeeCode)
                .Select(e => e.EmployeeCode)
                .FirstOrDefaultAsync();

            return Convert.ToInt32(lastCode) == 0 ? 3355 : Convert.ToInt32(lastCode) + 1;
        }
        public async Task<bool> EmailAvailable(string Email)
        {
            bool emailExists = await _db.Employees
                .AnyAsync(e => e.Email.ToLower() == Email.ToLower());
            return emailExists;

        }


    }
}
