using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly ApplicationDbContext _db;
        public SalaryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IQueryable<Salaries>> GetAllSalaries()
        {
            return await Task.FromResult(_db.Salary.Include(s => s.Employee).AsQueryable());
        }

        public async Task<Salaries?> GetSalaryById(int? id)
        {
            if (id == null || id == 0) return null;
            return await _db.Salary.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Salaries> AddSalary(Salaries salary)
        {
            if (salary == null) throw new ArgumentNullException(nameof(salary));
            _db.Salary.Add(salary);
            await _db.SaveChangesAsync();
            return salary;
        }

        public async Task<Salaries?> EditSalary(int? id, Salaries updatedSalary)
        {
            if (updatedSalary == null) throw new ArgumentNullException(nameof(updatedSalary));
            var existing = await _db.Salary.FindAsync(id);
            if (existing == null) return null;
            existing.BaseSalary = updatedSalary.BaseSalary;
            existing.Bonus = updatedSalary.Bonus;
            existing.Deductions = updatedSalary.Deductions;
            existing.EmployeeCode = updatedSalary.EmployeeCode;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<Salaries?> DeleteSalary(int? id)
        {
            var existing = await _db.Salary.FindAsync(id);
            if (existing == null) return null;
            _db.Salary.Remove(existing);
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<Salaries?> GetByEmployeeCode(string employeeCode)
        {
            if (string.IsNullOrEmpty(employeeCode)) return null;
            return await _db.Salary.Include(s => s.Employee).FirstOrDefaultAsync(s => s.EmployeeCode == employeeCode);
        }
    }
}
