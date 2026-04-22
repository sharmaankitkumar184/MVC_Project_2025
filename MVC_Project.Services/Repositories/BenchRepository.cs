using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;

namespace MVC_Project.Services.Repositories
{
    public class BenchRepository : IBenchRepository
    {
        private readonly ApplicationDbContext _db;

        public BenchRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IQueryable<Employee>> GetBenchEmployees()
        {
            return await Task.FromResult(
                _db.Employees
                    .Include(e => e.Department)
                    .Where(e => e.Department != null && e.Department.Name == "Bench")
                    .AsQueryable()
            );
        }

        public async Task<Employee?> GetBenchEmployeeById(int? id)
        {
            if (id == null || id == 0)
                throw new ArgumentNullException(nameof(id), "EmployeeId cannot be null");

            return await _db.Employees
                .Include(e => e.Department)
                .Include(e => e.Address)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id && e.Department != null && e.Department.Name == "Bench");
        }

        public async Task AssignEmployeeToDepartment(int employeeId, int departmentId)
        {
            if (employeeId == 0) throw new ArgumentOutOfRangeException(nameof(employeeId));
            if (departmentId == 0) throw new ArgumentOutOfRangeException(nameof(departmentId));

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var department = await _db.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);
            if (department == null)
                throw new KeyNotFoundException("Department not found");

            employee.DepartmentId = departmentId;
            await _db.SaveChangesAsync();
        }
    }
}

