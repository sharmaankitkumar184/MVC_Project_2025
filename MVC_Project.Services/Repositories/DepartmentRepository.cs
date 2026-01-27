using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;


namespace MVC_Project.Services.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {

        private readonly ApplicationDbContext _db;

        public DepartmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IQueryable<Department>> GetAllDepartment()
        {
            return _db.Departments.Include(e=>e.Employees).Include(p=>p.Projects);
        }

        public async Task<Department?> GetDepartmentById(int? id)
        {
            if (id == 0 || id == null)
            {
                throw new ArgumentNullException(nameof(id), "Department Id cannot be null");
            }
            var deptdata = _db.Departments.Include(e => e.Employees).Include(p => p.Projects).FirstOrDefault(i => i.Id == id);

            return deptdata;
        }
        public async Task<Department> AddDepartment(Department department)
        {
            if (department == null) throw new ArgumentNullException(nameof(department), "Department cannot be null");
            _db.Departments.Add(department);
            await _db.SaveChangesAsync();
            return department;
        }


        public async Task<Department?> EditDepartment(int? id, Department updatedDepartment)
        {
            if (updatedDepartment == null)
                throw new ArgumentNullException(nameof(updatedDepartment));

            var department = await _db.Departments.FindAsync(id);

            if (department == null)
                return null;

            department.Name = updatedDepartment.Name;
            department.Description = updatedDepartment.Description;
            department.IsActive = updatedDepartment.IsActive;
            department.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return department;
        }

        public async Task<Department> DeleteDepartment(int? id)
        {
            if (id == null) throw new ArgumentNullException("Department ID cannot be null");

            await ReassignEmployeesToBenchAsync(id);
            var departmentdata = await _db.Departments.FirstOrDefaultAsync(i => i.Id == id);
            if (departmentdata != null)
            {
                _db.Departments.Remove(departmentdata);
            }
            await _db.SaveChangesAsync();
            return departmentdata;
        }
        public async Task<Department> GetBenchDepartmentAsync()
        {
            return await _db.Departments
                .FirstAsync(d => d.Name == "Bench");
        }
        public async Task ReassignEmployeesToBenchAsync(int? departmentId)
        {
            var bench = await GetBenchDepartmentAsync();

            var employees = await _db.Employees
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();
            if (employees.Count > 0)

            {
                foreach (var employee in employees)
                {
                    employee.DepartmentId = bench.Id;
                    
                }
                await _db.SaveChangesAsync();
            }

        }
    }
}
