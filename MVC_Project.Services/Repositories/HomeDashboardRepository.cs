using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Data;

namespace MVC_Project.Services.Repositories
{
    public class HomeDashboardRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeDashboardRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Employee>> GetRecentEmployees(int count = 5)
        {
            return await _db.Employees
                .Include(e => e.Department)
                .Include(e => e.Address)
                .Include(e => e.Salary)
                .OrderByDescending(e => e.Id)
                .Take(count)
                .ToListAsync();
        }

        public async Task<(List<string> DepartmentNames, List<int> ProjectCounts)> GetProjectCountByDepartment()
        {
            var departments = await _db.Departments
                .Include(d => d.Projects)
                .ToListAsync();

            var names = departments.Select(d => d.Name).ToList();
            var counts = departments.Select(d => d.Projects?.Count ?? 0).ToList();

            return (names, counts);
        }

        public async Task<int> GetEmployeeCount()
        {
            return await _db.Employees.CountAsync();
        }

        public async Task<int> GetDepartmentCount()
        {
            return await _db.Departments.CountAsync();
        }

        public async Task<int> GetProjectCount()
        {
            return await _db.Projects.CountAsync();
        }
    }
}
