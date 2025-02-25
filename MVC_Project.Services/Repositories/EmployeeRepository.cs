using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;
using System.Collections.Generic;

namespace MVC_Project.Services.Repositories
{
    public class EmployeeRepository :IEmployeeRepository
    {

        private readonly ApplicationDbContext _db;

        public EmployeeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployee()
        {

            return await Task.FromResult(_db.Employees.Include(e => e.Department).Include(f=>f.Address).Include(s => s.Salary).ToList());

        }

        public async Task<Employee?> GetEmployeeById(int? id)
        {
            if (id == 0 || id == null)
            {
                throw new ArgumentNullException(nameof(id), "EmployeeId cannot be null");
            }
            var employee = await _db.Employees.Include(s=>s.Salary).Include(s => s.Department).Include(s => s.Address)
           .FirstOrDefaultAsync(m => m.Id == id);
            return employee;
        }


        public async Task<Employee> AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> EditEmployee(int? id, Employee employee)
        {
            try
            {
                _db.Employees.Update(employee);
                await _db.SaveChangesAsync();
                return employee;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
                {
                    throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Employee> DeleteEmployee(int? id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee != null)
            {
                _db.Employees.Remove(employee);
            }

            await _db.SaveChangesAsync();
            return employee;
        }
        private bool EmployeeExists(int id)
        {
            return _db.Employees.Any(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee?>> SearchEmployee(string empName)
        {
            if(empName == null)
            {
                throw new ArgumentNullException(nameof(empName), "Employee name cannot be null");
            }
            IList<Employee> employee = await _db.Employees
                                .Where(e => e.Name.ToLower().Contains(empName.ToLower())).ToListAsync();
            return employee;
        }

    }
}
