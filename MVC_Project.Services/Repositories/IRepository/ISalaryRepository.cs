using MVC_Project.Models.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
    public interface ISalaryRepository
    {
        Task<IQueryable<Salaries>> GetAllSalaries();
        Task<Salaries?> GetSalaryById(int? id);
        Task<Salaries> AddSalary(Salaries salary);
        Task<Salaries?> EditSalary(int? id, Salaries updatedSalary);
        Task<Salaries?> DeleteSalary(int? id);
        Task<Salaries?> GetByEmployeeCode(string employeeCode);
    }
}
